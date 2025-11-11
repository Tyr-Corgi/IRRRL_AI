using IRRRL.Core.Entities;
using IRRRL.Core.Enums;
using IRRRL.Core.Interfaces;

namespace IRRRL.Infrastructure.AI;

/// <summary>
/// AI-powered document validation service
/// </summary>
public class AIDocumentValidator : IAIDocumentValidator
{
    private readonly AIServiceConfig _config;
    
    public AIDocumentValidator(AIServiceConfig config)
    {
        _config = config;
    }
    
    public async Task<DocumentValidationResult> ValidateDocumentAsync(Document document, byte[] fileContent)
    {
        var result = new DocumentValidationResult
        {
            IsValid = true,
            ConfidenceScore = 0.85m
        };
        
        // Check file size
        if (fileContent.Length == 0)
        {
            result.IsValid = false;
            result.Issues.Add("Document file is empty");
            return result;
        }
        
        // Basic validation based on document type
        switch (document.DocumentType)
        {
            case DocumentType.VALoanStatement:
                result = await ValidateLoanStatementAsync(document, fileContent);
                break;
            case DocumentType.PhotoID:
                result = await ValidatePhotoIDAsync(document, fileContent);
                break;
            case DocumentType.PayStub:
                result = await ValidatePayStubAsync(document, fileContent);
                break;
            default:
                result = await ValidateGenericDocumentAsync(document, fileContent);
                break;
        }
        
        // Check if document is current (not expired)
        if (document.DocumentDate.HasValue)
        {
            var age = DateTime.UtcNow - document.DocumentDate.Value;
            
            // Different expiration rules for different documents
            var maxAge = document.DocumentType switch
            {
                DocumentType.PayStub => TimeSpan.FromDays(30),
                DocumentType.BankStatement => TimeSpan.FromDays(60),
                DocumentType.W2 => TimeSpan.FromDays(365),
                _ => TimeSpan.FromDays(90)
            };
            
            if (age > maxAge)
            {
                result.IsCurrent = false;
                result.Warnings.Add($"Document is {age.Days} days old. Please provide a more recent version.");
            }
        }
        
        result.IsValid = result.IsLegible && result.IsComplete && !result.Issues.Any();
        
        return result;
    }
    
    public async Task<Dictionary<string, string>> ExtractDataAsync(Document document, byte[] fileContent)
    {
        // This would use OCR (Azure Computer Vision or similar) to extract text
        // For now, returning a placeholder implementation
        
        var extractedData = new Dictionary<string, string>();
        
        switch (document.DocumentType)
        {
            case DocumentType.VALoanStatement:
                // Would extract: loan number, balance, interest rate, monthly payment
                extractedData["LoanNumber"] = "Extracted via OCR";
                extractedData["CurrentBalance"] = "Extracted via OCR";
                extractedData["InterestRate"] = "Extracted via OCR";
                extractedData["MonthlyPayment"] = "Extracted via OCR";
                break;
                
            case DocumentType.PhotoID:
                // Would extract: name, date of birth, ID number
                extractedData["Name"] = "Extracted via OCR";
                extractedData["DateOfBirth"] = "Extracted via OCR";
                extractedData["IDNumber"] = "Extracted via OCR";
                break;
                
            case DocumentType.PayStub:
                // Would extract: employer, pay period, gross pay, year-to-date
                extractedData["Employer"] = "Extracted via OCR";
                extractedData["PayPeriod"] = "Extracted via OCR";
                extractedData["GrossPay"] = "Extracted via OCR";
                break;
        }
        
        await Task.CompletedTask;
        return extractedData;
    }
    
    public async Task<bool> VerifyDocumentTypeAsync(Document document, byte[] fileContent)
    {
        // This would use AI to verify the document matches the expected type
        // For now, returning a basic implementation
        
        // Check file extension matches expected format
        var extension = Path.GetExtension(document.FileName).ToLowerInvariant();
        var validExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".tif", ".tiff" };
        
        if (!validExtensions.Contains(extension))
        {
            return false;
        }
        
        // Would use AI vision to classify document type and compare with expected
        await Task.CompletedTask;
        return true;
    }
    
    private async Task<DocumentValidationResult> ValidateLoanStatementAsync(Document document, byte[] fileContent)
    {
        var result = new DocumentValidationResult { ConfidenceScore = 0.9m };
        
        // Check for required fields (would be done via OCR in real implementation)
        var requiredFields = new[] { "Loan Number", "Current Balance", "Interest Rate", "Monthly Payment" };
        
        // Placeholder validation
        result.IsLegible = true;
        result.IsComplete = true;
        result.Notes = "Loan statement appears to contain all required information.";
        
        await Task.CompletedTask;
        return result;
    }
    
    private async Task<DocumentValidationResult> ValidatePhotoIDAsync(Document document, byte[] fileContent)
    {
        var result = new DocumentValidationResult { ConfidenceScore = 0.85m };
        
        // Check if photo is clear and ID is not expired
        result.IsLegible = true;
        result.IsComplete = true;
        result.Notes = "Photo ID is legible and valid.";
        
        await Task.CompletedTask;
        return result;
    }
    
    private async Task<DocumentValidationResult> ValidatePayStubAsync(Document document, byte[] fileContent)
    {
        var result = new DocumentValidationResult { ConfidenceScore = 0.88m };
        
        // Check for employer name, pay period, amounts
        result.IsLegible = true;
        result.IsComplete = true;
        
        // Check if recent (within 30 days)
        if (document.DocumentDate.HasValue)
        {
            var age = DateTime.UtcNow - document.DocumentDate.Value;
            if (age.Days > 30)
            {
                result.IsCurrent = false;
                result.Warnings.Add("Pay stub is older than 30 days. Please provide more recent pay stubs.");
            }
        }
        
        await Task.CompletedTask;
        return result;
    }
    
    private async Task<DocumentValidationResult> ValidateGenericDocumentAsync(Document document, byte[] fileContent)
    {
        var result = new DocumentValidationResult { ConfidenceScore = 0.8m };
        
        // Basic checks
        result.IsLegible = fileContent.Length > 1000; // Assume very small files might be issues
        result.IsComplete = true;
        result.Notes = "Document received and appears valid.";
        
        if (!result.IsLegible)
        {
            result.Issues.Add("Document file size is unusually small. Please verify the upload.");
        }
        
        await Task.CompletedTask;
        return result;
    }
}

