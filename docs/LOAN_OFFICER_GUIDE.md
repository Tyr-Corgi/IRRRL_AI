# 📋 Loan Officer Dashboard Guide

## Overview

The Loan Officer Dashboard is a comprehensive tool for managing IRRRL applications from submission through underwriting preparation. This guide covers all features and workflows.

---

## 🎯 Key Features

### 1. **Dashboard** (`/loanofficer/dashboard`)
- **Application Overview**: See all applications at a glance
- **Statistics Cards**: Track new, in-progress, ready-for-review, and completed apps
- **Search & Filter**: Find applications by veteran name, app number, or loan number
- **Quick Actions**: Jump directly to application reviews

### 2. **Application Detail Page** (`/loanofficer/application/{id}`)
Comprehensive review page with 5 tabs:

#### **Overview Tab**
- Complete veteran information (personal, contact, address, military service)
- Property details and current loan information
- Quick contact buttons (call/email veteran)
- Loan savings potential calculator

#### **Documents Tab**
- **AI-Generated Checklist**: Automatically creates personalized document list based on veteran's submission
- **Document Tracking**: Mark documents as received, request from veteran
- **Progress Bar**: Visual tracking of document collection
- **Upload History**: Track all uploaded files
- **Quick Actions**: Email document requests, print checklist, view VA guidelines

**Standard IRRRL Documents:**
- Certificate of Eligibility (COE)
- Most Recent Mortgage Statement
- Proof of Income (2 pay stubs or benefit statements)
- Bank Statements (last 2 months)
- Government-Issued ID
- Homeowner's Insurance Declaration Page

#### **Action Items Tab**
- **AI-Generated Tasks**: Automatically creates action items based on application analysis
- **Priority System**: High/Medium/Low priority tasks
- **Due Dates**: Track overdue and upcoming tasks
- **Progress Tracking**: Mark tasks complete as you go
- **AI Recommendations**: "Next Best Action" suggestions
- **Issue Flagging**: AI highlights potential problems

**Example AI-Generated Tasks:**
- Verify COE disability rating for funding fee calculation
- Request current mortgage statement
- Calculate Net Tangible Benefit
- Verify occupancy history
- Review bankruptcy/foreclosure declarations

#### **NTB Calculator Tab**
- **Interactive Calculator**: Compare current vs. proposed loan terms
- **Automatic Pre-Population**: Loads data from veteran's application
- **Real-Time Calculations**:
  - Rate reduction analysis
  - Monthly payment and savings
  - VA funding fee calculation (with disability waiver)
  - Recoupment period
  - NTB compliance verification

**VA NTB Requirements:**
- Fixed-to-Fixed: Must have 0.5% rate reduction
- ARM-to-Fixed: Must have lower payment
- All: Recoupment period ≤36 months

#### **Notes Tab**
- **Internal Communication**: Document all interactions and progress
- **Note Types**: General, Veteran Contact, Document Status, Calculation, Underwriter, Issue
- **Timeline View**: Chronological history of all notes
- **Quick Templates**: Pre-written notes for common scenarios
- **Important Flagging**: Mark critical notes for underwriter

---

## 🔄 Complete Workflow

### Step 1: New Application Arrives
1. Application appears on dashboard with "New" status
2. Click "Review" to open Application Detail page

### Step 2: Initial Review (Overview Tab)
1. Review veteran's information for completeness
2. Verify military service and VA eligibility
3. Check current loan details
4. Note any red flags or missing information

### Step 3: Document Collection (Documents Tab)
1. Click "AI Generate List" to create personalized document checklist
2. Review required documents based on application specifics
3. Request missing documents via email
4. Mark documents as received when veteran submits them
5. Upload documents to file

### Step 4: Action Items (Action Items Tab)
1. Review AI-generated action items
2. Work through high-priority tasks first
3. Add manual tasks as needed
4. Mark tasks complete as you finish them
5. Use "Regenerate with AI" after major updates

### Step 5: Calculate NTB (NTB Calculator Tab)
1. Verify pre-populated loan details
2. Enter proposed new interest rate
3. Adjust loan amount if funding fee is rolled in
4. Click "Calculate NTB"
5. Verify application meets NTB requirements
6. Document assumptions in Notes tab

### Step 6: Document Progress (Notes Tab)
1. Add note for each veteran contact
2. Document calculation results
3. Note any issues or special circumstances
4. Leave notes for underwriter

### Step 7: Update Status
1. Click "Update Status" button
2. Progress through statuses:
   - **New** → **In Progress** → **Documents Needed** → **Ready for Review** → **Underwriting** → **Completed**
3. Add status notes as needed

### Step 8: Prepare for Underwriting
1. Verify all required documents received (Documents Tab)
2. Confirm all high-priority action items complete (Action Items Tab)
3. Verify NTB compliance (NTB Calculator Tab)
4. Add final underwriter note (Notes Tab)
5. Update status to "Ready for Review"

---

## 💡 Best Practices

### Document Collection
- Request all documents early in process
- Follow up within 48 hours if no response
- Use AI to identify case-specific document needs
- Verify document authenticity and completeness

### Communication
- Document every veteran contact in Notes
- Use Quick Templates for common scenarios
- Mark important items for underwriter review
- Keep notes professional and detailed

### Eligibility Verification
- Always verify COE is current
- Check disability rating for funding fee waiver
- Confirm previous occupancy of property
- Review credit for bankruptcy/foreclosure declarations

### NTB Calculation
- Use conservative interest rate estimates
- Document all assumptions in Notes
- Verify recoupment period is under 36 months
- For fixed-to-fixed, ensure 0.5%+ rate reduction

### AI Utilization
- Regenerate action items after receiving new information
- Use AI document checklist for each application
- Review AI recommendations but apply professional judgment
- Document why you deviate from AI suggestions

---

## 🎨 Status Management

### Application Statuses
| Status | Meaning | Next Action |
|--------|---------|-------------|
| **New** | Just submitted | Begin initial review |
| **In Progress** | Actively working | Collect docs, verify info |
| **Documents Needed** | Waiting on veteran | Follow up on outstanding docs |
| **Ready for Review** | Complete, needs final check | Final review before UW |
| **Underwriting** | Sent to underwriter | Wait for UW decision |
| **Completed** | Approved/closed | Archive |

---

## 🔧 Features Coming Soon

- **File Upload**: Direct document upload functionality
- **Email Integration**: Send document requests directly from Documents Tab
- **PDF Generation**: Auto-generate VA forms from application data
- **Credit Report Integration**: Automatic credit pull and analysis
- **Task Automation**: Auto-complete certain action items based on document receipt
- **Reporting**: Generate productivity and pipeline reports

---

## 🆘 Troubleshooting

### "Documents Needed Count Not Updating"
- Refresh the page
- Check that required documents are properly marked

### "AI Generate List Not Working"
- Ensure OpenAI API key is configured in appsettings
- Check network connectivity

### "NTB Calculator Showing Wrong Results"
- Verify all input fields are filled correctly
- Check that rates are entered as percentages (4.5, not 0.045)
- Ensure loan amounts don't include commas

### "Can't Update Status"
- Verify you have LoanOfficer role
- Check that application is not locked by another user

---

## 📞 Support

For technical issues or feature requests, contact:
- **Development Team**: [support email]
- **System Admin**: [admin email]

---

## 🔐 Security & Compliance

- All veteran SSNs are encrypted in database
- Notes are only visible to loan officers and underwriters
- Document uploads are scanned for malware
- Audit trail maintained for all status changes
- GLBA and VA privacy requirements enforced

---

**Version**: 1.0  
**Last Updated**: November 2025  
**Author**: IRRRL Development Team

