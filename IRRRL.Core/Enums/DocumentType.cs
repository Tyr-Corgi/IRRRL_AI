namespace IRRRL.Core.Enums;

/// <summary>
/// Types of documents in the IRRRL process
/// </summary>
public enum DocumentType
{
    // Uploaded documents from veteran
    VALoanStatement = 0,
    CertificateOfEligibility = 1,
    PhotoID = 2,
    HomeownersInsurance = 3,
    PropertyTaxInfo = 4,
    
    // Additional documents for cash-out
    PayStub = 10,
    W2 = 11,
    TaxReturn = 12,
    BankStatement = 13,
    Appraisal = 14,
    
    // Generated documents
    VA_Form_26_8923 = 100,
    VA_Form_26_1820 = 101,
    Form_1003 = 102,
    LoanEstimate = 103,
    ClosingDisclosure = 104,
    InterestRateReductionWorksheet = 105,
    RecoupmentPeriodCalculation = 106,
    NetTangibleBenefitCertification = 107,
    FundingFeeDisclosure = 108,
    OccupancyCertification = 109,
    
    // Other
    Other = 999
}

