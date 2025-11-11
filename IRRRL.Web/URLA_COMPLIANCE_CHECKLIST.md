# URLA (Form 1003) Compliance Checklist for IRRRL Application

## Overview
This document compares our current IRRRL application against the Uniform Residential Loan Application (URLA/Form 1003) requirements.

**Note:** While IRRRLs are streamlined refinances and may not require ALL URLA fields, we should collect the minimum required information for VA compliance and best practices.

---

## ✅ Currently Collected

### Section 1a: Borrower Information
| Field | Status | Location |
|-------|--------|----------|
| First Name | ✅ Collected | Personal Info Step |
| Last Name | ✅ Collected | Personal Info Step |
| SSN | ✅ Collected (Encrypted) | Personal Info Step |
| Date of Birth | ✅ Collected | Personal Info Step |
| Phone Number | ✅ Collected | Personal Info Step |
| Email | ✅ Collected | Personal Info Step |

### Section 2a: Current Property & Loan
| Field | Status | Location |
|-------|--------|----------|
| Property Address | ✅ Collected | Property Info Step |
| City, State, ZIP | ✅ Collected | Property Info Step |
| Property Type | ✅ Collected (dropdown) | Property Info Step |
| Occupancy Type | ✅ Collected (dropdown) | Property Info Step |
| Current Loan Amount | ✅ Collected | Current Loan Step |
| Current Interest Rate | ✅ Collected | Current Loan Step |
| Current Monthly Payment | ✅ Collected | Current Loan Step |
| VA Loan Number | ✅ Collected | Current Loan Step |
| Original Loan Date | ✅ Collected | Current Loan Step |

### Section 5: Demographics (HMDA)
| Field | Status | Location |
|-------|--------|----------|
| Ethnicity | ✅ Collected (Optional) | Demographics Step |
| Race | ✅ Collected (Optional) | Demographics Step |
| Gender | ✅ Collected (Optional) | Demographics Step |

---

## ⚠️ Missing URLA Fields

### Section 1a: Borrower Information (Additional)
| Field | Required? | Priority | Notes |
|-------|-----------|----------|-------|
| Marital Status | Yes | HIGH | Needed for joint applications |
| Citizenship Status | Yes | HIGH | VA loan requirement |
| Number of Dependents | No | MEDIUM | For qualification purposes |
| Ages of Dependents | No | LOW | Optional detail |
| Current Address | Yes | HIGH | Where borrower lives NOW |
| Years at Current Address | Yes | MEDIUM | Stability indicator |
| Previous Address | Conditional | MEDIUM | If < 2 years at current |
| Mailing Address | No | LOW | If different from current |
| Alternate Phone | No | LOW | Secondary contact |
| Language Preference | No | MEDIUM | SCIF requirement (2023+) |

### Section 1b: Employment & Income
| Field | Required? | Priority | Notes |
|-------|-----------|----------|-------|
| **For IRRRL: NOT REQUIRED** | N/A | N/A | No income verification for rate/term |
| (Skip unless cash-out) | - | - | - |

### Section 2: Financial Info - Assets
| Field | Required? | Priority | Notes |
|-------|-----------|----------|-------|
| **For IRRRL: NOT REQUIRED** | N/A | N/A | No asset verification for rate/term |
| (Skip unless cash-out) | - | - | - |

### Section 2: Financial Info - Liabilities
| Field | Required? | Priority | Notes |
|-------|-----------|----------|-------|
| **For IRRRL: NOT REQUIRED** | N/A | N/A | No credit check for rate/term |
| (Skip unless cash-out) | - | - | - |

### Section 2: Real Estate Owned
| Field | Required? | Priority | Notes |
|-------|-----------|----------|-------|
| Other Properties | Conditional | LOW | Only if multiple VA loans |

### Section 3: Loan & Property Info
| Field | Required? | Priority | Notes |
|-------|-----------|----------|-------|
| Loan Purpose | Yes | HIGH | Should be "Refinance" |
| Property will be | Yes | HIGH | Primary/Secondary/Investment |
| Property acquired date | No | MEDIUM | When they bought it |
| Original Cost | No | MEDIUM | Purchase price |
| Improvements/Repairs | No | LOW | Cost of renovations |
| Current Liens | Yes | HIGH | How many mortgages |
| Property Description | No | LOW | More detail on property |

### Section 4: Declarations
| Field | Required? | Priority | Notes |
|-------|-----------|----------|-------|
| Outstanding Judgments | Yes | HIGH | Legal issues |
| Bankruptcy (past 7 yrs) | Yes | HIGH | Financial history |
| Foreclosure (past 7 yrs) | Yes | HIGH | Property history |
| Lawsuit/Litigation | Yes | MEDIUM | Legal proceedings |
| Loan Delinquency | Yes | HIGH | Payment history |
| In Default | Yes | HIGH | Current status |
| Alimony/Child Support | Yes | MEDIUM | Financial obligations |
| Down Payment Borrowed | No | LOW | Not applicable for refi |
| Co-Maker/Endorser | No | LOW | Guarantor info |
| U.S. Citizen | Yes | HIGH | VA requirement |
| Permanent Resident | Conditional | HIGH | If not citizen |
| Intent to Occupy | Yes | HIGH | Primary residence? |

### Section 6: Military Service (VA-Specific)
| Field | Required? | Priority | Notes |
|-------|-----------|----------|-------|
| Branch of Service | Yes | HIGH | Army, Navy, Air Force, etc. |
| Service Dates | Yes | HIGH | Start/End dates |
| Service Status | Yes | HIGH | Active/Veteran/Reserve |
| VA Case Number | No | MEDIUM | If previously used VA benefit |
| Disability Rating | No | MEDIUM | For funding fee exemption |
| Service Number | No | LOW | Old military ID |

### Section 7: Acknowledgments
| Field | Required? | Priority | Notes |
|-------|-----------|----------|-------|
| Electronic Consent | Yes | HIGH | E-signature agreement |
| Information Accuracy | Yes | HIGH | Borrower attestation |
| Credit Report Auth | Yes | HIGH | Permission to pull credit |
| Property Appraisal | Conditional | LOW | Not required for IRRRL |

---

## 📋 Recommended Additions (Prioritized)

### PRIORITY 1: Critical Missing Fields (Add Now)

1. **Borrower Citizenship & Marital Status**
   - U.S. Citizen (Yes/No/Permanent Resident)
   - Marital Status (Single/Married/Separated/Divorced/Widowed)
   - Spouse Name (if married)

2. **Current Address Information**
   - Current Street Address
   - City, State, ZIP
   - Years at Address
   - Rent or Own
   - Monthly Rent/Mortgage Payment

3. **Previous Address** (if < 2 years at current)
   - Street Address
   - City, State, ZIP
   - Years at Address

4. **Property Refinancing**
   - Estimated Property Value (we have this)
   - Number of Units (1-4)
   - Year Built
   - Property Acquired Date

5. **Military Service (VA-Specific)**
   - Branch of Service
   - Service Dates (From/To)
   - Current Status (Active/Veteran/Reserves)
   - Disability Rating (for fee waiver)

6. **Declarations**
   - Outstanding judgments?
   - Bankruptcy (past 7 years)?
   - Foreclosure (past 7 years)?
   - Party to lawsuit?
   - Loan obligations in default?
   - Intent to occupy property?

---

### PRIORITY 2: Important But Optional

7. **Contact Preferences**
   - Language Preference
   - Best Time to Call
   - Alternate Phone

8. **Co-Borrower Information** (if applicable)
   - Same fields as primary borrower
   - Only if joint application

9. **Property Details**
   - Number of Stories
   - Garage (Yes/No, # of Cars)
   - HOA (Yes/No, $ per month)

---

### PRIORITY 3: Nice to Have

10. **Additional Context**
    - Reason for Refinance (beyond rate reduction)
    - Estimated Closing Date Preference
    - Current Lender Name
    - Current Loan Servicer

---

## 🎯 IRRRL-Specific Simplifications

The VA IRRRL is a **streamlined refinance**, meaning:

### ✅ NOT REQUIRED:
- Income verification
- Employment history
- Asset documentation
- Full credit report (in many cases)
- Property appraisal
- Detailed financial statements

### ✅ STILL REQUIRED:
- Basic borrower identification
- Current loan information
- Property information
- Occupancy certification
- Military service verification
- Basic declarations (bankruptcy, foreclosure)
- Citizenship status

---

## 📊 Current vs. Target Comparison

| Category | Current Fields | URLA Standard | IRRRL Minimum | Recommendation |
|----------|---------------|---------------|---------------|----------------|
| Personal Info | 6 | 15+ | 8-10 | Add 4-5 fields |
| Current Address | 0 | 6 | 4 | Add full section |
| Military Service | 0 | 6 | 4 | Add full section |
| Property Details | 5 | 12+ | 6-7 | Add 2-3 fields |
| Current Loan | 6 | 8 | 6 | Complete ✅ |
| Declarations | 0 | 15+ | 6-8 | Add critical ones |
| Demographics | 3 | 3 | 0 (optional) | Complete ✅ |

---

## 🚀 Implementation Plan

### Phase 1: Critical Compliance (Do Now)
1. Add Marital Status & Citizenship
2. Add Current Address section
3. Add Military Service section
4. Add Critical Declarations (bankruptcy, foreclosure, occupancy)

### Phase 2: Enhanced Data Collection
5. Add Property Details (year built, units, acquired date)
6. Add Co-Borrower support (if married)
7. Add Previous Address (conditional)

### Phase 3: User Experience Improvements
8. Add Language Preference
9. Add Contact Preferences
10. Add Reason for Refinance

---

## 📝 Validation Rules Needed

| Field | Validation |
|-------|------------|
| SSN | Format: XXX-XX-XXXX (encrypted) |
| Date of Birth | Age >= 18 years |
| Phone | Format: (XXX) XXX-XXXX |
| Email | Valid email format |
| Citizenship | Required selection |
| Marital Status | Required selection |
| Current Address | Required, < 2 years = need previous |
| Property Address | Must match VA loan records |
| Military Branch | Required dropdown |
| Service Dates | End date >= Start date |
| Occupancy | Must certify primary residence |

---

## 🔍 Data Mapping: Our Model → URLA Sections

| Our Step | URLA Section | Completeness |
|----------|-------------|--------------|
| Personal Info | Section 1a (partial) | 60% |
| Current Loan | Section 2a | 95% |
| Property Info | Section 3 (partial) | 70% |
| Demographics | Section 5 | 100% |
| **MISSING** | Section 1b (Employment) | N/A (not needed) |
| **MISSING** | Section 2b (Assets) | N/A (not needed) |
| **MISSING** | Section 4 (Declarations) | 0% |
| **MISSING** | Section 6 (Military) | 0% |
| **MISSING** | Section 7 (Acknowledgments) | 0% |

---

## ✅ Next Steps

1. **Review this checklist** with stakeholders
2. **Prioritize which fields to add** (recommend all Priority 1)
3. **Update form model** with new fields
4. **Create new form steps** (Military Service, Declarations, Current Address)
5. **Add validation rules**
6. **Update database schema** to store new fields
7. **Test thoroughly** before deployment

---

## 📚 References

- [Fannie Mae URLA Form 1003](https://singlefamily.fanniemae.com/delivering/uniform-mortgage-data-program/uniform-residential-loan-application)
- [VA IRRRL Handbook (VA Pamphlet 26-7)](https://www.benefits.va.gov/homeloans/)
- [HMDA Demographic Data Collection](https://www.consumerfinance.gov/rules-policy/regulations/1003/)

---

**Last Updated:** November 11, 2024

