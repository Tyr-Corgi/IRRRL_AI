# 🧪 Quick Test Checklist - IRRRL Streamlining

## Pre-Test Setup ✅

- [ ] Application builds successfully
- [ ] Database is created/updated
- [ ] Can access http://localhost:5000
- [ ] Can login as veteran@irrrl.local

---

## Critical Features to Test

### ✅ 1. "Same as Property Address" Checkbox
**Location:** Step 2 - Current Address

- [ ] Checkbox appears at top of form
- [ ] Checking it populates address fields
- [ ] Checking it disables address fields (grayed out)
- [ ] Unchecking re-enables fields
- [ ] Data persists when toggling

**Status:** ☐ PASS ☐ FAIL

---

### ✅ 2. Conditional Spouse Fields
**Location:** Step 1 - Personal Info

**Test A: Single/Unmarried**
- [ ] Select "Unmarried"
- [ ] Spouse fields DO NOT appear
- [ ] Can proceed to next step

**Test B: Married**
- [ ] Select "Married"
- [ ] Blue alert box appears
- [ ] Spouse First Name field appears
- [ ] Spouse Last Name field appears
- [ ] Fields are marked required (*)
- [ ] Cannot proceed without filling them

**Test C: Toggle**
- [ ] Switch from Married → Unmarried
- [ ] Spouse fields disappear
- [ ] Switch back to Married
- [ ] Spouse fields reappear

**Status:** ☐ PASS ☐ FAIL

---

### ✅ 3. Conditional Previous Address
**Location:** Step 2 - Current Address

**Test A: Long-term resident (>2 years)**
- [ ] Years: 5, Months: 0
- [ ] Previous address section DOES NOT appear
- [ ] Can proceed to next step

**Test B: Recent resident (<2 years)**
- [ ] Years: 1, Months: 6
- [ ] Previous address section APPEARS
- [ ] Previous address fields are present
- [ ] Can fill in previous address
- [ ] Can proceed to next step

**Test C: Exactly 2 years**
- [ ] Years: 2, Months: 0
- [ ] Previous address DOES NOT appear (24 months is the threshold)

**Status:** ☐ PASS ☐ FAIL

---

### ✅ 4. Streamlined Property Info
**Location:** Step 5 - Property Information

**Verify These Fields EXIST:**
- [ ] Property Address
- [ ] City
- [ ] State (dropdown)
- [ ] ZIP Code
- [ ] "Get Estimated Home Value" button
- [ ] Property Type (dropdown)
- [ ] Occupancy (dropdown)

**Verify These Fields DO NOT EXIST:**
- [ ] ✗ Number of Units
- [ ] ✗ Year Built
- [ ] ✗ Date Acquired

**Status:** ☐ PASS ☐ FAIL

---

### ✅ 5. Streamlined Declarations
**Location:** Step 6 - Declarations

**Verify Exactly 3 Questions Appear:**
- [ ] Intent to occupy property? (Yes/No)
- [ ] Bankruptcy in past 7 years? (Yes/No)
- [ ] Foreclosure in past 7 years? (Yes/No)

**Verify These Questions DO NOT APPEAR:**
- [ ] ✗ Outstanding judgments
- [ ] ✗ Party to lawsuit
- [ ] ✗ Delinquent federal debt
- [ ] ✗ Alimony/child support

**Test Conditional Fields:**
- [ ] Select "Yes" for Bankruptcy
- [ ] Bankruptcy Type dropdown appears
- [ ] Discharge Date field appears
- [ ] Can fill them in

**Status:** ☐ PASS ☐ FAIL

---

### ✅ 6. Review Page Accuracy
**Location:** Step 8 - Review & Submit

**Verify Correct Display:**
- [ ] Personal info shows correctly
- [ ] Spouse name appears ONLY if married
- [ ] Current address shows correctly
- [ ] Previous address shows ONLY if provided
- [ ] Military service shows correctly
- [ ] Current loan shows correctly
- [ ] Property info shows (without Units/Year/Date)
- [ ] Declarations show ONLY 3 items
- [ ] Demographics show ONLY if filled

**Test Edit Buttons:**
- [ ] Click "Edit" on any section
- [ ] Returns to that step
- [ ] Data is still there
- [ ] Can modify and return to review

**Status:** ☐ PASS ☐ FAIL

---

### ✅ 7. Remaining Term Calculator
**Location:** Step 4 - Current Loan

**Test Calculations:**
- [ ] Years: 5, Months: 0 → Shows "~25 years (300 months)"
- [ ] Years: 10, Months: 0 → Shows "~20 years (240 months)"
- [ ] Years: 0, Months: 6 → Shows "~30 years (354 months)"
- [ ] Years: 30, Months: 0 → Shows "~0 years (0 months)"

**Status:** ☐ PASS ☐ FAIL

---

### ✅ 8. Estimated Home Value Button
**Location:** Step 5 - Property Information

**Test Behavior:**
- [ ] Button shows "📊 Get Estimated Home Value"
- [ ] Click without address → Shows error message
- [ ] Fill in complete address
- [ ] Click button → Shows "Estimating..." with spinner
- [ ] After ~2 seconds → Success message appears
- [ ] Shows estimated value (e.g., $350,000)
- [ ] Green alert box displays

**Status:** ☐ PASS ☐ FAIL

---

## 🎯 Overall Test Results

### Summary Table

| Feature | Status | Notes |
|---------|--------|-------|
| Same as Property Checkbox | ☐ Pass ☐ Fail | |
| Conditional Spouse Fields | ☐ Pass ☐ Fail | |
| Conditional Prev Address | ☐ Pass ☐ Fail | |
| Streamlined Property Info | ☐ Pass ☐ Fail | |
| Streamlined Declarations | ☐ Pass ☐ Fail | |
| Review Page Accuracy | ☐ Pass ☐ Fail | |
| Remaining Term Calculator | ☐ Pass ☐ Fail | |
| Estimated Value Button | ☐ Pass ☐ Fail | |

### Final Verdict

**Overall Status:** ☐ PASS ☐ FAIL

**Critical Issues:**
1. _______________________________________________
2. _______________________________________________
3. _______________________________________________

**Minor Issues:**
1. _______________________________________________
2. _______________________________________________

**Additional Comments:**
_______________________________________________
_______________________________________________

---

## 📝 Quick Issue Report Template

If you find a bug, report it like this:

**Issue #1: [Short Description]**
- **Severity:** ☐ Critical ☐ Major ☐ Minor
- **Location:** Step X - [Section Name]
- **Steps to Reproduce:**
  1. _______________
  2. _______________
  3. _______________
- **Expected:** _______________
- **Actual:** _______________
- **Screenshot:** (if available)

---

## ✅ Ready to Test!

1. **Start:** Run `run-tests.ps1` or `run-tests.cmd`
2. **Or Manual:** `dotnet run --project IRRRL.Web`
3. **Login:** veteran@irrrl.local / Veteran@123!
4. **Test:** Follow this checklist top to bottom
5. **Report:** Document any issues found

**Good luck! 🚀**

