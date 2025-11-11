using Azure.AI.OpenAI;
using IRRRL.Core.Entities;
using IRRRL.Core.Enums;
using IRRRL.Core.Interfaces;
using System.Text.Json;

namespace IRRRL.Infrastructure.AI;

/// <summary>
/// AI-powered action item generator using Azure OpenAI
/// </summary>
public class AIActionItemGenerator : IAIActionItemGenerator
{
    private readonly AIServiceConfig _config;
    
    public AIActionItemGenerator(AIServiceConfig config)
    {
        _config = config;
    }
    
    public async Task<List<ActionItem>> GenerateInitialActionItemsAsync(IRRRLApplication application)
    {
        var actionItems = new List<ActionItem>();
        
        // Determine required documents based on application type
        var requiredDocs = GetRequiredDocuments(application);
        
        int orderIndex = 1;
        
        // Generate action items for each required document
        foreach (var docType in requiredDocs)
        {
            actionItems.Add(new ActionItem
            {
                IRRRLApplicationId = application.Id,
                Title = $"Collect {GetDocumentDisplayName(docType)}",
                Description = GetDocumentDescription(docType, application),
                Priority = GetDocumentPriority(docType, application),
                Status = ActionItemStatus.Pending,
                RelatedDocumentType = docType,
                OrderIndex = orderIndex++,
                GeneratedByAI = true,
                AIReasoning = $"Required document for {application.ApplicationType} IRRRL application",
                EstimatedMinutes = EstimateCollectionTime(docType)
            });
        }
        
        // Add verification action items
        if (application.ApplicationType == ApplicationType.RateAndTerm)
        {
            actionItems.Add(new ActionItem
            {
                IRRRLApplicationId = application.Id,
                Title = "Verify Veteran's Occupancy Certification",
                Description = "Confirm that the veteran has signed the occupancy certification form indicating they previously or currently occupy the property.",
                Priority = ActionItemPriority.High,
                Status = ActionItemStatus.Pending,
                OrderIndex = orderIndex++,
                GeneratedByAI = true,
                AIReasoning = "Required VA occupancy verification for IRRRL",
                EstimatedMinutes = 5
            });
        }
        
        // Add cash-out specific items
        if (application.ApplicationType == ApplicationType.CashOut)
        {
            actionItems.Add(new ActionItem
            {
                IRRRLApplicationId = application.Id,
                Title = "Obtain Loan Officer Approval for Cash-Out",
                Description = $"Cash-out amount of ${application.CashOutAmount:N2} requires manual review and approval before proceeding with full documentation.",
                Priority = ActionItemPriority.Critical,
                Status = ActionItemStatus.Pending,
                OrderIndex = 1, // Make this first
                GeneratedByAI = true,
                AIReasoning = "Cash-out applications require manual approval per policy",
                EstimatedMinutes = 30
            });
        }
        
        return actionItems;
    }
    
    public async Task<List<ActionItem>> GenerateDocumentActionItemsAsync(IRRRLApplication application, List<DocumentType> missingDocuments)
    {
        var actionItems = new List<ActionItem>();
        
        foreach (var docType in missingDocuments)
        {
            actionItems.Add(new ActionItem
            {
                IRRRLApplicationId = application.Id,
                Title = $"URGENT: Missing {GetDocumentDisplayName(docType)}",
                Description = $"This document is still required to proceed. {GetDocumentDescription(docType, application)}",
                Priority = ActionItemPriority.High,
                Status = ActionItemStatus.Pending,
                RelatedDocumentType = docType,
                GeneratedByAI = true,
                AIReasoning = "Document identified as missing during processing",
                DueDate = DateTime.UtcNow.AddDays(3),
                EstimatedMinutes = EstimateCollectionTime(docType)
            });
        }
        
        return actionItems;
    }
    
    public async Task<List<ActionItem>> GenerateFollowUpActionItemsAsync(IRRRLApplication application, Document document, string? issues = null)
    {
        var actionItems = new List<ActionItem>();
        
        if (!document.IsValidated || !document.IsComplete || !document.IsLegible)
        {
            var issueDescription = issues ?? "Document requires attention.";
            
            if (!document.IsLegible)
            {
                issueDescription = "Document image is not clear enough to read. Please request a higher quality scan or photo.";
            }
            else if (!document.IsComplete)
            {
                issueDescription = "Document appears to be incomplete or missing pages. Please obtain the complete document.";
            }
            else if (!document.IsCurrent)
            {
                issueDescription = "Document is outdated. Please obtain a more recent version.";
            }
            
            actionItems.Add(new ActionItem
            {
                IRRRLApplicationId = application.Id,
                Title = $"Re-collect: {document.FileName}",
                Description = $"The uploaded document has quality issues that need to be resolved: {issueDescription}",
                Priority = ActionItemPriority.High,
                Status = ActionItemStatus.Pending,
                RelatedDocumentId = document.Id,
                RelatedDocumentType = document.DocumentType,
                GeneratedByAI = true,
                AIReasoning = "AI detected quality issues with uploaded document",
                DueDate = DateTime.UtcNow.AddDays(2),
                EstimatedMinutes = 15
            });
        }
        
        return actionItems;
    }
    
    public async Task UpdateActionItemPrioritiesAsync(IRRRLApplication application)
    {
        // This would analyze the current state and update priorities
        // For now, we'll implement basic logic
        
        foreach (var actionItem in application.ActionItems.Where(a => a.Status == ActionItemStatus.Pending))
        {
            // Increase priority if past due date
            if (actionItem.DueDate.HasValue && actionItem.DueDate.Value < DateTime.UtcNow)
            {
                if (actionItem.Priority < ActionItemPriority.Critical)
                {
                    actionItem.Priority = ActionItemPriority.High;
                }
            }
            
            // Critical documents get higher priority
            if (actionItem.RelatedDocumentType.HasValue)
            {
                var criticalDocs = new[] 
                { 
                    DocumentType.VALoanStatement, 
                    DocumentType.CertificateOfEligibility 
                };
                
                if (criticalDocs.Contains(actionItem.RelatedDocumentType.Value) && 
                    actionItem.Priority < ActionItemPriority.High)
                {
                    actionItem.Priority = ActionItemPriority.High;
                }
            }
        }
        
        await Task.CompletedTask;
    }
    
    private List<DocumentType> GetRequiredDocuments(IRRRLApplication application)
    {
        var docs = new List<DocumentType>
        {
            DocumentType.VALoanStatement,
            DocumentType.CertificateOfEligibility,
            DocumentType.PhotoID,
            DocumentType.HomeownersInsurance,
            DocumentType.PropertyTaxInfo
        };
        
        // Add cash-out required documents
        if (application.ApplicationType == ApplicationType.CashOut)
        {
            docs.AddRange(new[]
            {
                DocumentType.PayStub,
                DocumentType.W2,
                DocumentType.BankStatement
            });
            
            // Add tax returns if self-employed or large cash-out
            if (application.CashOutAmount > 50000)
            {
                docs.Add(DocumentType.TaxReturn);
            }
        }
        
        return docs;
    }
    
    private string GetDocumentDisplayName(DocumentType docType)
    {
        return docType switch
        {
            DocumentType.VALoanStatement => "Current VA Loan Statement",
            DocumentType.CertificateOfEligibility => "Certificate of Eligibility (COE)",
            DocumentType.PhotoID => "Photo ID",
            DocumentType.HomeownersInsurance => "Homeowners Insurance Policy",
            DocumentType.PropertyTaxInfo => "Property Tax Information",
            DocumentType.PayStub => "Recent Pay Stubs (Last 30 days)",
            DocumentType.W2 => "W-2 Forms (Last 2 years)",
            DocumentType.TaxReturn => "Tax Returns (Last 2 years)",
            DocumentType.BankStatement => "Bank Statements (Last 2 months)",
            DocumentType.Appraisal => "Property Appraisal",
            _ => docType.ToString()
        };
    }
    
    private string GetDocumentDescription(DocumentType docType, IRRRLApplication application)
    {
        return docType switch
        {
            DocumentType.VALoanStatement => "Most recent mortgage statement showing loan number, current balance, interest rate, and monthly payment.",
            DocumentType.CertificateOfEligibility => "Can be obtained electronically through the VA or from veteran's records.",
            DocumentType.PhotoID => "Driver's license or government-issued photo identification.",
            DocumentType.HomeownersInsurance => "Current homeowners insurance policy or declaration page with coverage amounts.",
            DocumentType.PropertyTaxInfo => "Recent property tax bill or statement for escrow calculation.",
            DocumentType.PayStub => "Most recent pay stubs covering the last 30 days of employment.",
            DocumentType.W2 => "W-2 forms from the last 2 years for income verification.",
            DocumentType.TaxReturn => "Complete tax returns (all schedules) for the last 2 years.",
            DocumentType.BankStatement => "Bank statements for all accounts from the last 2 months.",
            _ => "Please collect this document from the veteran."
        };
    }
    
    private ActionItemPriority GetDocumentPriority(DocumentType docType, IRRRLApplication application)
    {
        // Critical documents needed to proceed
        if (docType == DocumentType.VALoanStatement || 
            docType == DocumentType.CertificateOfEligibility)
        {
            return ActionItemPriority.Critical;
        }
        
        // High priority for required documents
        if (docType == DocumentType.PhotoID || 
            docType == DocumentType.HomeownersInsurance)
        {
            return ActionItemPriority.High;
        }
        
        return ActionItemPriority.Medium;
    }
    
    private int EstimateCollectionTime(DocumentType docType)
    {
        return docType switch
        {
            DocumentType.VALoanStatement => 10,
            DocumentType.CertificateOfEligibility => 15,
            DocumentType.PhotoID => 5,
            DocumentType.HomeownersInsurance => 10,
            DocumentType.PropertyTaxInfo => 10,
            DocumentType.PayStub => 15,
            DocumentType.W2 => 20,
            DocumentType.TaxReturn => 30,
            DocumentType.BankStatement => 15,
            DocumentType.Appraisal => 60,
            _ => 15
        };
    }
}

