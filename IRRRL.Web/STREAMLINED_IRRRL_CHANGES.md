# Streamlined IRRRL Changes

## What We're Removing for IRRRL Streamline

### ✂️ **Fields to Remove:**

#### 1. **Current Address Section - CONDITIONAL**
**Decision:** Keep the section BUT only show previous address if < 2 years
- ✅ Keep: Current address (needed for primary residence verification)
- ✅ Keep: "Same as property" checkbox (convenience)
- ✅ Keep: Years at address
- ✅ CONDITIONAL: Previous address (ONLY if < 2 years)

**Why:** VA needs to verify this is/was primary residence

---

#### 2. **Property Details - SIMPLIFY**
**Remove:**
- ❌ Year Built (nice to have, not required)
- ❌ Date Acquired (can get from loan origination)
- ❌ Number of Units (assume single family unless specified)

**Keep:**
- ✅ Property Address
- ✅ Property Type
- ✅ Occupancy Status

---

#### 3. **Declarations - KEEP CRITICAL ONLY**
**Remove:**
- ❌ Outstanding Judgments (not typically required for streamline)
- ❌ Party to Lawsuit (not typically required)
- ❌ Delinquent Federal Debt (VA will check anyway)
- ❌ Alimony/Child Support (no income verification needed)

**Keep:**
- ✅ Intent to Occupy (REQUIRED - primary residence certification)
- ✅ Bankruptcy (past 7 years) - VA needs this
- ✅ Foreclosure (past 7 years) - VA needs this

**Why:** IRRRL is streamlined - minimal financial review needed

---

#### 4. **Spouse Information - SIMPLIFY**
**Change:**
- Keep Marital Status
- ❌ Remove Spouse Name fields (not needed unless co-borrower)

**Why:** Solo applications don't need spouse details for streamline refi

---

### ✅ **What We're Keeping (Still Required):**

1. **Personal Info**
   - Name, DOB, SSN, Contact
   - Citizenship Status
   - Marital Status

2. **Current Address**
   - Where you live now
   - Years at address
   - Housing status (Own/Rent)

3. **Military Service** (VA-SPECIFIC - REQUIRED)
   - Branch, Dates, Status
   - Disability Rating (for fee waiver)

4. **Current Loan** (CORE IRRRL DATA)
   - Loan amount, rate, payment
   - VA loan number
   - Remaining term

5. **Property Info** (SIMPLIFIED)
   - Address, Type, Occupancy

6. **Declarations** (MINIMAL)
   - Occupancy intent
   - Bankruptcy/Foreclosure

7. **Demographics** (OPTIONAL HMDA)
   - Race, Ethnicity, Gender

---

## Summary of Changes

| Section | Before | After | Reduction |
|---------|--------|-------|-----------|
| Personal Info | 10 fields | 8 fields | -2 (spouse name) |
| Current Address | 12+ fields | 8 fields | -4 (prev address conditional) |
| Military Service | 7 fields | 7 fields | 0 (all required) |
| Current Loan | 6 fields | 6 fields | 0 (all required) |
| Property Info | 9 fields | 6 fields | -3 |
| Declarations | 10 fields | 3 fields | -7 |
| Demographics | 3 fields | 3 fields | 0 (optional) |
| **TOTAL** | **~57 fields** | **~41 fields** | **-16 fields (28% reduction)** |

---

## New Application Flow (Streamlined)

### 8 Steps → 8 Steps (same count, but simpler)

1. **Personal Info** - Name, DOB, SSN, Contact, Citizenship, Marital Status
2. **Current Address** - Where you live (+ previous if < 2 years)
3. **Military Service** - Branch, dates, status, disability
4. **Current Loan** - Existing VA loan details
5. **Property Info** - Address, type, occupancy (simplified)
6. **Declarations** - Occupancy, bankruptcy, foreclosure (simplified)
7. **Demographics** - Optional HMDA data
8. **Review & Submit** - Final review

---

## Implementation Plan

### Phase 1: Remove Spouse Name Fields ✅
- Update PersonalInfoStep.razor
- Remove conditional spouse fields
- Update model (keep MaritalStatus, remove spouse names)

### Phase 2: Simplify Property Info ✅
- Remove: NumberOfUnits, YearBuilt, PropertyAcquiredDate
- Keep: Address, Type, Occupancy, EstimatedValue

### Phase 3: Streamline Declarations ✅
- Keep only: IntendToOccupyProperty, HasBankruptcy, HasForeclosure
- Remove: Judgments, Lawsuit, Federal Debt, Alimony

### Phase 4: Conditional Previous Address ✅
- Already implemented - only shows if < 2 years

### Phase 5: Update Review Page ✅
- Remove removed fields from display
- Clean up declarations section

---

## Benefits of Streamlining

### ✅ **For Veterans:**
- **Faster application** - Less time to complete
- **Less confusion** - Only essential questions
- **True "streamline" experience** - Lives up to IRRRL promise

### ✅ **For Loan Officers:**
- **Faster processing** - Less data to review
- **Clear focus** - Only VA-required information
- **Less follow-up** - Fewer unnecessary questions

### ✅ **For Compliance:**
- **Still VA-compliant** - All required fields present
- **HMDA-compliant** - Demographics still collected
- **Audit-ready** - Essential documentation maintained

---

## What About Cash-Out Later?

**Future Enhancement:** Create separate flow for cash-out
- Can build on this foundation
- Add back: Employment, Income, Assets, Full Declarations
- Use conditional logic: `if (loanType === "CashOut")` show extra fields

**For Now:** Focus on streamline perfection! 🎯

