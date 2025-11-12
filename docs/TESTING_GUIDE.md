# IRRRL Application Testing Guide

## Quick Start Testing

### 1. Start the Application

**Option A: Using Visual Studio**
- Open `IRRRL.sln`
- Press F5 or click "Run"

**Option B: Using Command Line (CMD)**
```cmd
cd "C:\Mac\Home\Desktop\Repos\IRRRL AI"
dotnet run --project IRRRL.Web
```

**Expected Output:**
```
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
```

### 2. Access the Application
Open your browser and go to: **http://localhost:5000**

---

## Test Accounts

### Veteran Account (For Testing Application Flow)
- **Email:** `veteran@irrrl.local`
- **Password:** `Veteran@123!`
- **Role:** Veteran (Can submit applications)

### Admin Account (For Backend Testing Later)
- **Email:** `admin@irrrl.local`
- **Password:** `Admin@123!`
- **Role:** Administrator

---

## Testing Scenarios

## 🧪 Scenario 1: Single Veteran - Living at Property (Simplest Path)

**Goal:** Test the fastest, most streamlined application path

**Steps:**

1. **Login** as `veteran@irrrl.local`

2. **Navigate** to New Application (should auto-navigate or click "New Application")

3. **Step 1: Personal Info**
   - First Name: `John`
   - Last Name: `Smith`
   - Date of Birth: `01/15/1980`
   - SSN: `123-45-6789`
   - Phone: `(555) 123-4567`
   - Email: `john.smith@example.com`
   - Citizenship: `U.S. Citizen`
   - Marital Status: `Unmarried` ✅
   - **✓ Verify:** Spouse fields DO NOT appear
   - Click **Next**

4. **Step 2: Current Address**
   - **✓ Check:** `☑ Same as property address` ✅
   - **✓ Verify:** Address fields auto-fill and become disabled
   - Years at Address: `5`
   - Months at Address: `0`
   - Housing Status: `Own`
   - Monthly Payment: `1500`
   - **✓ Verify:** Previous address section DOES NOT appear (>2 years)
   - Click **Next**

5. **Step 3: Military Service**
   - Branch: `Army`
   - Status: `Veteran (Honorably Discharged)`
   - Start Date: `01/2000`
   - End Date: `01/2005`
   - VA Disability Rating: `30` (or leave blank)
   - Click **Next**

6. **Step 4: Current Loan**
   - Current Loan Amount: `250000`
   - Interest Rate: `4.5`
   - Monthly Payment: `1500`
   - Years Ago: `5`
   - Months Ago: `0`
   - **✓ Verify:** Remaining term shows "~25 years (300 months)"
   - VA Loan Number: `12-3456789`
   - Click **Next**

7. **Step 5: Property Info**
   - Property Address: `123 Main Street`
   - City: `San Diego`
   - State: `CA`
   - ZIP: `92101`
   - **Optional:** Click "Get Estimated Home Value" ✅
   - **✓ Verify:** Shows estimated value after ~2 seconds
   - Property Type: `Single Family Home`
   - Occupancy: `Primary Residence`
   - **✓ Verify:** NO fields for Units, Year Built, Acquired Date
   - Click **Next**

8. **Step 6: Declarations**
   - **✓ Verify:** Only 3 questions appear (not 10)
   - Intent to Occupy: `Yes - Primary Residence`
   - Bankruptcy: `No`
   - Foreclosure: `No`
   - Click **Next**

9. **Step 7: Demographics (Optional)**
   - **Optional:** Fill out or skip
   - Click **Next**

10. **Step 8: Review & Submit**
    - **✓ Verify:** All entered data displays correctly
    - **✓ Verify:** Spouse info NOT shown
    - **✓ Verify:** Only 3 declarations shown
    - **✓ Verify:** Property shows only Address, Type, Occupancy (no Units/Year/Date)
    - Click **Submit Application**

**Expected Result:** ✅ Success message, redirect to confirmation

---

## 🧪 Scenario 2: Married Veteran - Different Address (Complex Path)

**Goal:** Test conditional fields (spouse info, previous address)

**Steps:**

1. **Login** as `veteran@irrrl.local`

2. **New Application**

3. **Step 1: Personal Info**
   - First Name: `Jane`
   - Last Name: `Doe`
   - Date of Birth: `06/20/1985`
   - SSN: `987-65-4321`
   - Phone: `(555) 987-6543`
   - Email: `jane.doe@example.com`
   - Citizenship: `U.S. Citizen`
   - Marital Status: `Married` ✅
   - **✓ Verify:** Spouse fields APPEAR with blue alert box
   - Spouse First Name: `Bob`
   - Spouse Last Name: `Doe`
   - Click **Next**

4. **Step 2: Current Address**
   - **✓ DO NOT check** "Same as property"
   - Street: `456 Oak Avenue`
   - City: `Los Angeles`
   - State: `CA`
   - ZIP: `90001`
   - Years at Address: `1` ✅
   - Months at Address: `3` ✅
   - Housing Status: `Rent`
   - Monthly Payment: `2000`
   - **✓ Verify:** Previous Address section APPEARS (< 2 years) ✅
   - **Previous Address:**
     - Street: `789 Pine Street`
     - City: `San Francisco`
     - State: `CA`
     - ZIP: `94102`
     - Years: `2`
     - Months: `0`
   - Click **Next**

5. **Step 3: Military Service**
   - Branch: `Navy`
   - Status: `Active Duty`
   - Start Date: `06/2005`
   - End Date: (leave blank - still active)
   - VA Disability Rating: (leave blank)
   - Click **Next**

6. **Step 4: Current Loan**
   - Current Loan Amount: `400000`
   - Interest Rate: `5.0`
   - Monthly Payment: `2200`
   - Years Ago: `3`
   - Months Ago: `6`
   - **✓ Verify:** Remaining term shows "~26 years (318 months)"
   - VA Loan Number: `98-7654321`
   - Click **Next**

7. **Step 5: Property Info**
   - Property Address: `321 Beach Blvd`
   - City: `Oceanside`
   - State: `CA`
   - ZIP: `92054`
   - Property Type: `Townhouse`
   - Occupancy: `Primary Residence`
   - Click **Next**

8. **Step 6: Declarations**
   - Intent to Occupy: `Yes - Primary Residence`
   - Bankruptcy: `Yes` ✅
   - **✓ Verify:** Bankruptcy details fields APPEAR
   - Bankruptcy Type: `Chapter 7`
   - Discharge Date: `01/15/2020`
   - Foreclosure: `No`
   - Click **Next**

9. **Step 7: Demographics**
   - Skip
   - Click **Next**

10. **Step 8: Review & Submit**
    - **✓ Verify:** Spouse name appears (Bob Doe)
    - **✓ Verify:** Current AND previous address shown
    - **✓ Verify:** Bankruptcy shows "Yes - Chapter 7"
    - Click **Submit Application**

**Expected Result:** ✅ Success message

---

## 🧪 Scenario 3: Edge Cases & Validation Testing

### Test A: Property Address Auto-Copy Toggle
1. Start application
2. Reach Current Address step
3. **Check** "Same as property address"
   - **✓ Verify:** Fields populate from property info
   - **✓ Verify:** Fields become disabled (grayed out)
4. **Uncheck** the box
   - **✓ Verify:** Fields become editable again
   - **✓ Verify:** Data remains (doesn't clear)

### Test B: Conditional Field Visibility
1. Start application
2. Step 1: Select "Married"
   - **✓ Verify:** Spouse fields appear immediately
3. Change to "Unmarried"
   - **✓ Verify:** Spouse fields disappear immediately
4. Change back to "Married"
   - **✓ Verify:** Spouse fields reappear (data may be cleared)

### Test C: Previous Address Threshold
1. Current Address step
2. Enter Years: `2`, Months: `0`
   - **✓ Verify:** Previous address DOES NOT appear (exactly 24 months)
3. Change to Years: `1`, Months: `11`
   - **✓ Verify:** Previous address DOES NOT appear (23 months, but < 24)
4. Change to Years: `1`, Months: `10`
   - **✓ Verify:** Previous address section APPEARS (22 months)

### Test D: Required Field Validation
1. Try to advance each step without filling required fields
2. **✓ Verify:** Red validation messages appear
3. **✓ Verify:** Cannot proceed to next step
4. Fill in required fields
5. **✓ Verify:** Can now proceed

### Test E: Remaining Loan Term Calculator
1. Current Loan step
2. Enter Years Ago: `5`, Months Ago: `0`
   - **✓ Verify:** Shows "~25 years (300 months)"
3. Change to Years: `0`, Months: `1`
   - **✓ Verify:** Shows "~30 years (359 months)"
4. Change to Years: `30`, Months: `0`
   - **✓ Verify:** Shows "~0 years (0 months)"

### Test F: Estimated Home Value
1. Property Info step
2. Leave address fields empty
3. Click "Get Estimated Home Value"
   - **✓ Verify:** Error message appears
4. Fill in complete address
5. Click "Get Estimated Home Value"
   - **✓ Verify:** Button shows "Estimating..." with spinner
   - **✓ Verify:** After ~2 seconds, shows success with value
   - **✓ Verify:** Green alert box displays estimate

### Test G: Back Button Navigation
1. Complete several steps
2. Click "← Back" button
3. **✓ Verify:** Returns to previous step
4. **✓ Verify:** Previously entered data is retained
5. Click "Next" to proceed forward again
6. **✓ Verify:** Can continue where you left off

---

## 🐛 Common Issues & Troubleshooting

### Issue: Application won't start
**Symptom:** Error when running `dotnet run`
**Fix:** 
```cmd
cd "C:\Mac\Home\Desktop\Repos\IRRRL AI"
dotnet clean
dotnet build
dotnet run --project IRRRL.Web
```

### Issue: Database errors on startup
**Symptom:** "No such table: AspNetUsers"
**Fix:**
```cmd
cd "C:\Mac\Home\Desktop\Repos\IRRRL AI"
dotnet ef database update --project IRRRL.Infrastructure --startup-project IRRRL.Web
```

### Issue: Can't login
**Symptom:** "Invalid email or password"
**Fix:** Database may need to be recreated
```cmd
# Delete database files
del IRRRL_Dev.db
del IRRRL_Dev.db-shm
del IRRRL_Dev.db-wal

# Recreate database
dotnet ef database update --project IRRRL.Infrastructure --startup-project IRRRL.Web
```

### Issue: Checkbox doesn't copy address
**Symptom:** "Same as property" checkbox does nothing
**Cause:** Need to complete Property Info step first
**Fix:** This is expected - property address must be entered in Step 5 before it can be copied in Step 2. This is a known limitation of the multi-step wizard.

### Issue: Page is blank after login
**Symptom:** White screen after successful login
**Fix:** 
1. Open browser console (F12)
2. Check for JavaScript errors
3. Try clearing browser cache (Ctrl+Shift+Delete)
4. Try incognito/private browsing mode

---

## ✅ Testing Checklist

### Functionality Tests
- [ ] Can login as veteran
- [ ] Can start new application
- [ ] Step 1: Personal info saves correctly
- [ ] Step 1: Spouse fields conditional on marital status
- [ ] Step 2: "Same as property" checkbox works
- [ ] Step 2: Previous address only shows when < 2 years
- [ ] Step 3: Military service saves correctly
- [ ] Step 4: Remaining term calculates correctly
- [ ] Step 5: Property info saves correctly
- [ ] Step 5: Estimated value button works (simulated)
- [ ] Step 6: Only 3 declarations appear
- [ ] Step 6: Bankruptcy details conditional
- [ ] Step 7: Demographics is optional
- [ ] Step 8: Review shows all correct data
- [ ] Step 8: Can edit by clicking "Edit" buttons
- [ ] Can submit application successfully

### Validation Tests
- [ ] Cannot skip required fields
- [ ] SSN format validation works
- [ ] Email format validation works
- [ ] Phone format validation works
- [ ] ZIP code format validation works
- [ ] Date fields validate properly
- [ ] Number fields validate ranges

### UI/UX Tests
- [ ] All steps display correctly
- [ ] Progress bar shows correct step
- [ ] Back button works on all steps
- [ ] Next button advances to correct step
- [ ] Forms are responsive (try resizing browser)
- [ ] Mobile view works (or simulate with browser tools)
- [ ] All text is readable
- [ ] No overlapping elements

### Browser Compatibility
- [ ] Works in Chrome
- [ ] Works in Edge
- [ ] Works in Firefox
- [ ] Works in Safari (if on Mac)

---

## 📊 Test Results Template

**Date:** __________
**Tester:** __________
**Browser:** __________
**Build Version:** __________

| Scenario | Status | Notes |
|----------|--------|-------|
| Scenario 1: Single Veteran | ☐ Pass ☐ Fail | |
| Scenario 2: Married Veteran | ☐ Pass ☐ Fail | |
| Scenario 3: Edge Cases | ☐ Pass ☐ Fail | |
| Validation Tests | ☐ Pass ☐ Fail | |
| UI/UX Tests | ☐ Pass ☐ Fail | |

**Overall Result:** ☐ PASS ☐ FAIL

**Issues Found:**
1. _______________________________________________
2. _______________________________________________
3. _______________________________________________

**Additional Comments:**
_______________________________________________
_______________________________________________

---

## 🚀 Ready to Test?

1. Start the application: `dotnet run --project IRRRL.Web`
2. Open browser: `http://localhost:5000`
3. Login: `veteran@irrrl.local` / `Veteran@123!`
4. Follow Scenario 1 first (simplest path)
5. Report any issues you find!

**Good luck testing! 🎯**

