# 🧪 Testing Environment - IRRRL Application

## Overview

This directory contains a complete testing environment for the streamlined IRRRL application. We've recently made significant improvements to focus on the core IRRRL streamline process.

---

## 🚀 Quick Start (3 Steps)

### Step 1: Run the Test Script
**Windows (PowerShell):**
```powershell
cd "C:\Mac\Home\Desktop\Repos\IRRRL AI\IRRRL.Web"
.\run-tests.ps1
```

**Windows (CMD):**
```cmd
cd "C:\Mac\Home\Desktop\Repos\IRRRL AI\IRRRL.Web"
run-tests.cmd
```

### Step 2: Start the Application
```cmd
dotnet run --project IRRRL.Web
```

### Step 3: Open Browser
Navigate to: **http://localhost:5000**

---

## 📚 Testing Documentation

### 1. **TESTING_GUIDE.md** (Comprehensive Manual)
- **What:** Complete testing manual with detailed scenarios
- **When to use:** First time testing or thorough testing session
- **Contains:**
  - 3 complete test scenarios (simple, complex, edge cases)
  - Step-by-step instructions
  - Expected results
  - Troubleshooting guide

### 2. **QUICK_TEST_CHECKLIST.md** (Fast Testing)
- **What:** Quick checkbox-based testing checklist
- **When to use:** Regression testing or quick verification
- **Contains:**
  - 8 critical features to verify
  - Simple pass/fail checklist
  - Issue reporting template

### 3. **SESSION_CHANGES_STREAMLINING.md** (What Changed)
- **What:** Documentation of recent streamlining changes
- **When to use:** Understanding what was modified
- **Contains:**
  - Complete list of changes
  - Before/after comparison
  - Field reduction stats (28% reduction!)

### 4. **STREAMLINED_IRRRL_CHANGES.md** (Planning Doc)
- **What:** Original planning document for streamlining
- **When to use:** Understanding the reasoning behind changes
- **Contains:**
  - Detailed field-by-field analysis
  - Compliance considerations
  - Future enhancement plans

---

## 🎯 What We're Testing

### New Features (Priority)
1. ✅ **"Same as Property Address" checkbox** - Auto-copies address
2. ✅ **Conditional spouse fields** - Only show if married
3. ✅ **Conditional previous address** - Only show if < 2 years
4. ✅ **Streamlined property info** - Removed 3 unnecessary fields
5. ✅ **Streamlined declarations** - Reduced from 10 to 3 questions

### Core Functionality (Still Need to Verify)
- Multi-step wizard navigation
- Form validation
- Data persistence between steps
- Review page accuracy
- Remaining term calculator
- Estimated home value (simulated API)

---

## 👤 Test Accounts

### Veteran Account
```
Email: veteran@irrrl.local
Password: Veteran@123!
Role: Veteran
```
**Use for:** Testing the application submission flow

### Admin Account
```
Email: admin@irrrl.local
Password: Admin@123!
Role: Administrator
```
**Use for:** Backend testing (future use)

---

## 🧪 Recommended Testing Order

### First Time? Start Here:
1. ✅ Run `run-tests.ps1` to verify build
2. ✅ Read **QUICK_TEST_CHECKLIST.md**
3. ✅ Test Scenario 1 from **TESTING_GUIDE.md** (simplest path)
4. ✅ Test critical features from checklist
5. ✅ Report any issues

### Regular Testing:
1. ✅ Run `run-tests.ps1`
2. ✅ Follow **QUICK_TEST_CHECKLIST.md**
3. ✅ Mark pass/fail for each feature
4. ✅ Report issues

### Deep Testing:
1. ✅ Complete all 3 scenarios in **TESTING_GUIDE.md**
2. ✅ Test all edge cases
3. ✅ Try to break it (validation, navigation, etc.)
4. ✅ Test in multiple browsers

---

## 📊 Current Test Status

**Last Updated:** [Date of testing]

| Feature | Status | Notes |
|---------|--------|-------|
| Build Compilation | ✅ Pass | No errors |
| Same as Property Checkbox | ⏳ Pending | Needs manual test |
| Conditional Spouse Fields | ⏳ Pending | Needs manual test |
| Conditional Prev Address | ⏳ Pending | Needs manual test |
| Streamlined Property | ⏳ Pending | Needs manual test |
| Streamlined Declarations | ⏳ Pending | Needs manual test |
| Review Page | ⏳ Pending | Needs manual test |
| Full Application Flow | ⏳ Pending | Needs manual test |

Legend:
- ✅ Pass - Working as expected
- ❌ Fail - Has issues
- ⏳ Pending - Not yet tested
- ⚠️ Warning - Works but has minor issues

---

## 🐛 Known Issues

*(Update this section as you find issues)*

### Critical Issues
None currently known

### Minor Issues
None currently known

### Future Enhancements
1. Property API integration (currently simulated)
2. AI integration for document processing
3. Email notifications
4. PDF generation for forms

---

## 🔧 Troubleshooting

### "Application won't start"
```cmd
cd "C:\Mac\Home\Desktop\Repos\IRRRL AI"
dotnet clean
dotnet build
dotnet run --project IRRRL.Web
```

### "Database errors"
```cmd
del IRRRL_Dev.db
del IRRRL_Dev.db-shm
del IRRRL_Dev.db-wal
dotnet ef database update --project IRRRL.Infrastructure --startup-project IRRRL.Web
```

### "Can't login"
- Verify you're using: `veteran@irrrl.local` / `Veteran@123!`
- Database may need recreation (see above)
- Check browser console for errors (F12)

### "Checkbox doesn't work"
- This is expected - property address must be filled in Step 5 first
- Known limitation of multi-step wizard
- Consider this a future enhancement

---

## 📝 Reporting Issues

When you find a bug, document it in this format:

```markdown
**Issue: [Short Title]**
- Severity: Critical/Major/Minor
- Location: Step X - Section Name
- Steps to Reproduce:
  1. Do this
  2. Then this
  3. See error
- Expected: What should happen
- Actual: What actually happens
- Screenshot: (attach if possible)
```

Save issues to a new file: `TEST_RESULTS_[DATE].md`

---

## ✅ Testing Checklist Summary

### Pre-Test
- [ ] Application builds without errors
- [ ] Database is created/migrated
- [ ] Can access http://localhost:5000
- [ ] Can login as test veteran

### Main Features (8 Critical Items)
- [ ] Same as property checkbox works
- [ ] Spouse fields conditional on marital status
- [ ] Previous address conditional on time at current
- [ ] Property info streamlined (no units/year/date)
- [ ] Declarations streamlined (only 3 questions)
- [ ] Review page shows correct data
- [ ] Remaining term calculator works
- [ ] Estimated value button works

### Complete Flow
- [ ] Can complete entire application start-to-finish
- [ ] Can submit application successfully
- [ ] No console errors in browser

---

## 🎉 Testing Complete!

Once all checkboxes are marked:
1. ✅ Create a test results file
2. ✅ Document any issues found
3. ✅ Update the "Current Test Status" table above
4. ✅ Celebrate! 🎊

**Questions?** Review the full **TESTING_GUIDE.md** for detailed help.

**Ready to test?** Start with **QUICK_TEST_CHECKLIST.md**!

