﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Pkurg.BPM.Entities;
using Pkurg.PWorld.Business.Permission;
using Pkurg.PWorldBPM.Business.BIZ;
using Pkurg.PWorldBPM.Business.Workflow;
using SourceCode.Workflow.Client;

public partial class Workflow_ApprovePage_A_OA_InstructionOfEToI : UPageBase
{
    InstructionOfEToI Aitems = new InstructionOfEToI();
    WF_WorkFlowInstance wf_WorkFlowInstance = new WF_WorkFlowInstance();
    BFApprovalRecord bfApproval = new BFApprovalRecord();
    string DefaultApprover = System.Configuration.ConfigurationManager.AppSettings["DefaultApprover"];
    public string FormID
    {
        get
        {
            return ViewState["FormID"].ToString();
        }
        set
        {
            ViewState["FormID"] = value;
        }
    }
    public string StartDeptId
    {
        get
        {
            return lbDeptCode.Text;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        //防止二次提交
        ScriptManager.RegisterOnSubmitStatement(Page, typeof(Page), "Go_disabled", "disabledButton('lbAgree');disabledButton('lbReject');disabledButton('lbSubmit');");
        ScriptManager.RegisterStartupScript(Page, typeof(Page), "Clear_disabled", "enableButton('lbAgree');enableButton('lbReject');enableButton('lbSubmit');", true);
        if (!IsPostBack)
        {
            ViewState["IsSubmit"] = false;
            if (!string.IsNullOrEmpty(Request.QueryString["sn"]))
            {
                WorkflowHelper.CurrentUser = "founder\\" + _BPMContext.CurrentUser.LoginId;
                sn.Value = Request.QueryString["sn"];
                WorklistItem listItem = WorkflowHelper.GetWorklistItemWithSN(sn.Value, "founder\\" + _BPMContext.CurrentUser.LoginId);
                taskID.Value = listItem.ID.ToString();
                nodeID.Value = listItem.ActivityInstanceDestination.ActID.ToString();
                nodeName.Value = listItem.ActivityInstanceDestination.Name;
                InitApproveOpinion(nodeName.Value);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                ViewState["FormID"] = _BPMContext.ProcInst.FormId;
                BindFormData();
            }
            else
            {
                ExceptionHander.GoToErrorPage();
            }
            ShowButton();
            InitCheckBoxList();
            InitCheckButton();
            InitLeader();
        }      
    }

    private void ShowButton()
    {
        if (string.IsNullOrEmpty(Request.QueryString["sn"]))
        {
            Options.Visible = false;
            UnOptions.Visible = false;
            ASOptions.Visible = false;
        }
        else
        {
            bool isAddSign = new Workflow_Common().IsAddSign(Request.QueryString["sn"], _BPMContext.CurrentUser.LoginId);
            if (isAddSign)
            {
                Options.Visible = false;
                UnOptions.Visible = false;
                ASOptions.Visible = true;
            }
            else
            {
                Options.Visible = true;
                UnOptions.Visible = true;
                ASOptions.Visible = false;
            }
        }
    }

    private void InitLeader()
    {
        string CompanyCode = BPMHelp.GetCompanyCodeByDeptID(StartDeptId);
        BFPmsUserRoleDepartment bfurd = new BFPmsUserRoleDepartment();
        DataTable dtDeptManager = bfurd.GetSelectRoleUser(StartDeptId, "部门负责人");
        DataTable dtPresident = bfurd.GetSelectRoleUser(CompanyCode, "总裁");
        DataTable dtEVP = bfurd.GetSelectRoleUser(CompanyCode, "常务副总裁");
        DataTable dtCFO = bfurd.GetSelectRoleUser(CompanyCode, "主管财务领导");
        DataTable dtChairman = bfurd.GetSelectRoleUser(CompanyCode, "董事长");

        if (StartDeptId.Contains("S377"))
        {
            trEVP.Visible = true;
        }
        if (dtDeptManager.Rows.Count != 0)
        {
            lbDeptManager.Text = "(" + dtDeptManager.Rows[0]["EmployeeName"].ToString() + ")审批";
        }
        if (dtCFO.Rows.Count != 0)
        {
            lbCFO.Text = "(" + dtCFO.Rows[0]["EmployeeName"].ToString() + ")审批";
        }
        else
        {
            lbCFO.Text = "尚未配置";
        }
        if (dtEVP.Rows.Count != 0)
        {
            lbEVP.Text = "(" + dtEVP.Rows[0]["EmployeeName"].ToString() + ")审批";
        }
        if (dtPresident.Rows.Count != 0)
        {
            lbPresident.Text = "(" + dtPresident.Rows[0]["EmployeeName"].ToString() + ")审批";
        }
        if (dtChairman.Rows.Count != 0)
        {
            lbChairman.Text = "(" + dtChairman.Rows[0]["EmployeeName"].ToString() + ")审批";
        }
    }

    private void InitCheckButton()
    {
        if (nodeName.Value == "流程审核员审核")
        {
            cbAP.Enabled = true;
            cbVP.Enabled = true;
            cbCFO.Enabled = true;
            cbEVP.Enabled = true;
            cbPresident.Enabled = true;
            cbChairman.Enabled = true;
            cbIsReport.Enabled = true;
            cblDirectors.Visible = true;
            lbDirectors.Visible = true;
            Countersign1.IsCanEdit = true;
        }
        else
        {
            Countersign1.IsCanEdit = false;
        }
    }

    private void InitCheckBoxList()
    {
        BFPmsUserRoleDepartment bfurd = new BFPmsUserRoleDepartment();
        string PresidentOfficeCode = BPMHelp.GetDeptIDByOtherIDAndName(StartDeptId, "总办会");
        DataTable dtLeaders = bfurd.GetSelectRoleUser(PresidentOfficeCode, "部门成员");

        foreach (DataRow user in dtLeaders.Rows)
        {
            ListItem li = new ListItem();
            li.Value = "K2:Founder\\" + user["LoginName"].ToString();
            li.Text = user["EmployeeName"].ToString();
            if (!cblDirectors.Items.Contains(li))
            {
                cblDirectors.Items.Add(li);
            }
        }

        if (!string.IsNullOrEmpty(Request.QueryString["id"]))
        {
            XmlDocument xmldoc = new XmlDocument();
            InstructionOfEToIInfo formDataInfo = Aitems.Get(FormID);
            if (!string.IsNullOrEmpty(formDataInfo.LeadersSelected))
            {
                xmldoc.LoadXml(formDataInfo.LeadersSelected);
            }

            XmlNode NodeLeaders = xmldoc.SelectSingleNode("//Directors");
            if (NodeLeaders != null && NodeLeaders.Attributes["ID"].Value.Length > 0)
            {
                foreach (string UserGuid in NodeLeaders.Attributes["ID"].Value.Split(','))
                {
                    for (int i = 0; i < cblDirectors.Items.Count; i++)
                    {
                        if (cblDirectors.Items[i].Value == UserGuid)
                        {
                            cblDirectors.Items[i].Selected = true;
                        }
                    }
                }
            }
            XmlNode NodeChairman = xmldoc.SelectSingleNode("//Chairman");
            if (NodeChairman != null && NodeChairman.Attributes["ID"].Value != "noapprovers")
            {
                cbChairman.Checked = true;
            }
            XmlNode NodePresident = xmldoc.SelectSingleNode("//President");
            if (NodePresident != null && NodePresident.Attributes["ID"].Value != "noapprovers")
            {
                cbPresident.Checked = true;
            }
            XmlNode NodeEVP = xmldoc.SelectSingleNode("//EVP");
            if (NodeEVP != null && NodeEVP.Attributes["ID"].Value != "noapprovers")
            {
                cbEVP.Checked = true;
            }
            XmlNode NodeCFO = xmldoc.SelectSingleNode("//CFO");
            if (NodeCFO != null && NodeCFO.Attributes["ID"].Value != "noapprovers")
            {
                cbCFO.Checked = true;
            }
            XmlNode NodeVP = xmldoc.SelectSingleNode("//VicePresident");
            if (NodeVP != null && NodeVP.Attributes["ID"].Value != "noapprovers")
            {
                cbVP.Checked = true;
            }
            XmlNode NodeAP = xmldoc.SelectSingleNode("//AssiPresident");
            if (NodeAP != null && NodeAP.Attributes["ID"].Value != "noapprovers")
            {
                cbAP.Checked = true;
            }
        }
        else
        {
            for (int i = 0; i < cblDirectors.Items.Count; i++)
            {
                cblDirectors.Items[i].Selected = false;
            }
        }
    }

    private void BindFormData()
    {
        try
        {
            InstructionOfEToIInfo obj = Aitems.Get(FormID);
            if (obj != null)
            {
                tbReportCode.Text = obj.FormID;
                tbDepartName.Text = obj.DeptName;
                lbDeptCode.Text = obj.DeptCode;
                tbDateTime.Text = obj.DateTime;
                tbUserName.Text = obj.UserName;
                tbMobile.Text = obj.Mobile;
                tbTitle.Text = obj.Title;
                tbContent.Text = obj.Content;
                cbIsReport.Checked = obj.IsReport == "1" ? true : false;
                cblSecurityLevel.SelectedValue = obj.SecurityLevel != null ? obj.SecurityLevel.ToString() : "-1";
                cblUrgenLevel.SelectedValue = obj.UrgenLevel != null ? obj.UrgenLevel.ToString() : "-1";
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //批准
    protected void Agree_Click(object sender, EventArgs e)
    {
        WorkFlowInstance workFlowInstance = wf_WorkFlowInstance.GetWorkFlowInstanceByFormId(FormID);
        bool IsSubmit = true;
        if (nodeName.Value == "流程审核员审核")
        {
            SaveWFParams();
            UpdateWFParams();
            SaveData(ViewState["FormID"].ToString(),"");
        }

        if(IsSubmit)
        {
            if (workFlowInstance != null)
            {
                UploadAttachments1.SaveAttachment(FormID);
                string action = "同意";
                bool isSuccess = WorkflowHelper.ApproveProcess(sn.Value, action, "founder\\" + _BPMContext.CurrentUser.LoginId);
                string Opinion = "";
                if (GetApproveOpinion(nodeName.Value) == null || GetApproveOpinion(nodeName.Value) == "")
                {
                    Opinion = "同意";
                }
                else
                {
                    Opinion = GetApproveOpinion(nodeName.Value);
                }
                string ApproveResult = "同意";
                string OpinionType = "";
                string IsSign = "0";
                string DelegateUserName = "";
                string DelegateUserCode = "";

                if (isSuccess && !(bool)ViewState["IsSubmit"])
                {
                    var appRecord = new Pkurg.PWorldBPM.Business.Sys.WF_Approval_Record()
                    {
                         ApprovalID = Guid.NewGuid().ToString(),
                         WFTaskID = int.Parse(taskID.Value),
                       FormID = FormID,
                        InstanceID = workFlowInstance.InstanceId,
                        Opinion = Opinion,
                        ApproveAtTime = DateTime.Now,
                        ApproveByUserCode = CurrentEmployee.EmployeeCode,
                        ApproveByUserName = CurrentEmployee.EmployeeName,
                        ApproveResult = ApproveResult,
                        OpinionType = OpinionType,
                        CurrentActiveName = nodeName.Value,
                        ISSign = IsSign,
                        CurrentActiveID= nodeID.Value,
                        DelegateUserName = DelegateUserName,
                        DelegateUserCode = DelegateUserCode,
                        CreateAtTime = DateTime.Now,
                        CreateByUserCode = CurrentEmployee.EmployeeCode,
                        CreateByUserName = CurrentEmployee.EmployeeName,
                        UpdateAtTime = DateTime.Now,
                        UpdateByUserCode = CurrentEmployee.EmployeeCode,
                        UpdateByUserName = CurrentEmployee.EmployeeName,
                        FinishedTime = DateTime.Now
                    };

                    if (bfApproval.AddApprovalRecord(appRecord))
                    {
                        ViewState["IsSubmit"] = true;
                        wf_WorkFlowInstance.UpdateStatus(workFlowInstance.WfInstanceId, "1", nodeID.Value, nodeName.Value, int.Parse(taskID.Value), null, CurrentEmployee);

                        ScriptManager.RegisterClientScriptBlock(this, typeof(string), "1", "alert('审批成功'); window.opener.location.href=window.opener.location.href; ", true);
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ajax", " window.opener=null; window.open('', '_self', '');window.close();", true);
                    }
                }
            }
        }
    }

    //拒绝
    protected void Reject_Click(object sender, EventArgs e)
    {
        WorkFlowInstance workFlowInstance = wf_WorkFlowInstance.GetWorkFlowInstanceByFormId(FormID);
        if (workFlowInstance != null)
        {
            UploadAttachments1.SaveAttachment(FormID);
            string action = "不同意";
            ChangeResultToUnAgree();
            bool isSuccess = WorkflowHelper.ApproveProcess(sn.Value, action, "founder\\" + _BPMContext.CurrentUser.LoginId);
            string Opinion = GetApproveOpinion(nodeName.Value);
            if (string.IsNullOrEmpty(Opinion))
            {
                Opinion = "不同意";
            }
            string ApproveResult = "不同意";
            string OpinionType = "";
            string IsSign = "0";
            string DelegateUserName = "";
            string DelegateUserCode = "";
            if (isSuccess && !(bool)ViewState["IsSubmit"])
            {
                NameValueCollection dataFields = new NameValueCollection();
                var appRecord = new Pkurg.PWorldBPM.Business.Sys.WF_Approval_Record()
                {
                     ApprovalID = Guid.NewGuid().ToString(),
                     WFTaskID = int.Parse(taskID.Value),
                   FormID = FormID,
                    InstanceID = workFlowInstance.InstanceId,
                    Opinion = Opinion,
                    ApproveAtTime = DateTime.Now,
                    ApproveByUserCode = CurrentEmployee.EmployeeCode,
                    ApproveByUserName = CurrentEmployee.EmployeeName,
                    ApproveResult = ApproveResult,
                    OpinionType = OpinionType,
                    CurrentActiveName = nodeName.Value,
                    ISSign = IsSign,
                    CurrentActiveID= nodeID.Value,
                    DelegateUserName = DelegateUserName,
                    DelegateUserCode = DelegateUserCode,
                    CreateAtTime = DateTime.Now,
                    CreateByUserCode = CurrentEmployee.EmployeeCode,
                    CreateByUserName = CurrentEmployee.EmployeeName,
                    UpdateAtTime = DateTime.Now,
                    UpdateByUserCode = CurrentEmployee.EmployeeCode,
                    UpdateByUserName = CurrentEmployee.EmployeeName,
                    FinishedTime = DateTime.Now
                };
                bfApproval.AddApprovalRecord(appRecord);
                ViewState["IsSubmit"] = true;
                wf_WorkFlowInstance.UpdateStatus(workFlowInstance.WfInstanceId, "1", nodeID.Value, nodeName.Value, int.Parse(taskID.Value), null, CurrentEmployee);

                ScriptManager.RegisterClientScriptBlock(this, typeof(string), "1", "alert('审批成功'); window.opener.location.href=window.opener.location.href; ", true);
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ajax", " window.opener=null; window.open('', '_self', '');window.close();", true);
            }
        }
    }

    //提交
    protected void Submit_Click(object sender, EventArgs e)
    {
        WorkFlowInstance workFlowInstance = wf_WorkFlowInstance.GetWorkFlowInstanceByFormId(FormID);

        if (workFlowInstance != null)
        {
            UploadAttachments1.SaveAttachment(FormID);
            bool isSuccess = WorkflowHelper.BackToPreApprover(sn.Value, "founder\\" + _BPMContext.CurrentUser.LoginId);

            string Opinion = GetApproveOpinion(nodeName.Value);
            if (string.IsNullOrEmpty(Opinion))
            {
                Opinion = "同意";
            }
            string ApproveResult = "同意";
            string OpinionType = "";
            string IsSign = "2";//会签步骤
            string DelegateUserName = "";
            string DelegateUserCode = "";
            if (isSuccess && !(bool)ViewState["IsSubmit"])
            {
                var appRecord = new Pkurg.PWorldBPM.Business.Sys.WF_Approval_Record()
                {
                     ApprovalID = Guid.NewGuid().ToString(),
                     WFTaskID = int.Parse(taskID.Value),
                   FormID = FormID,
                    InstanceID = workFlowInstance.InstanceId,
                    Opinion = Opinion,
                    ApproveAtTime = DateTime.Now,
                    ApproveByUserCode = CurrentEmployee.EmployeeCode,
                    ApproveByUserName = CurrentEmployee.EmployeeName,
                    ApproveResult = ApproveResult,
                    OpinionType = OpinionType,
                    CurrentActiveName = nodeName.Value,
                    ISSign = IsSign,
                    CurrentActiveID= nodeID.Value,
                    DelegateUserName = DelegateUserName,
                    DelegateUserCode = DelegateUserCode,
                    CreateAtTime = DateTime.Now,
                    CreateByUserCode = CurrentEmployee.EmployeeCode,
                    CreateByUserName = CurrentEmployee.EmployeeName,
                    UpdateAtTime = DateTime.Now,
                    UpdateByUserCode = CurrentEmployee.EmployeeCode,
                    UpdateByUserName = CurrentEmployee.EmployeeName,
                    FinishedTime = DateTime.Now
                };

                if (bfApproval.AddApprovalRecord(appRecord))
                {
                    wf_WorkFlowInstance.UpdateStatus(workFlowInstance.WfInstanceId, "1", nodeID.Value, nodeName.Value, int.Parse(taskID.Value), null, CurrentEmployee);

                    ScriptManager.RegisterClientScriptBlock(this, typeof(string), "1", "alert('审批成功'); window.opener.location.href=window.opener.location.href; ", true);
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ajax", " window.opener=null; window.open('', '_self', '');window.close();", true);
                }
            }
        }
    }

    private void UpdateWFParams()
    {
        NameValueCollection dataFields = new NameValueCollection();
        XmlDocument xmldoc = new XmlDocument();
        xmldoc.LoadXml(lblApprovers.Text);
        XmlNode NodeTemp;
        string LeaderTemp;

        NodeTemp = xmldoc.SelectSingleNode("//Chairman");
        if (NodeTemp != null)
        {
            LeaderTemp = string.IsNullOrEmpty(NodeTemp.Attributes["ID"].Value) ? "noapprovers" : NodeTemp.Attributes["ID"].Value;
        }
        else
        {
            LeaderTemp = "noapprovers";
        }
        dataFields.Add("Chairman", LeaderTemp);

        NodeTemp = xmldoc.SelectSingleNode("//President");
        if (NodeTemp != null)
        {
            LeaderTemp = string.IsNullOrEmpty(NodeTemp.Attributes["ID"].Value) ? "noapprovers" : NodeTemp.Attributes["ID"].Value;
        }
        else
        {
            LeaderTemp = "noapprovers";
        }
        dataFields.Add("President", LeaderTemp);

        NodeTemp = xmldoc.SelectSingleNode("//EVP");
        if (NodeTemp != null)
        {
            LeaderTemp = string.IsNullOrEmpty(NodeTemp.Attributes["ID"].Value) ? "noapprovers" : NodeTemp.Attributes["ID"].Value;
        }
        else
        {
            LeaderTemp = "noapprovers";
        }
        dataFields.Add("EVP", LeaderTemp);

        NodeTemp = xmldoc.SelectSingleNode("//CFO");
        if (NodeTemp != null)
        {
            LeaderTemp = string.IsNullOrEmpty(NodeTemp.Attributes["ID"].Value) ? "noapprovers" : NodeTemp.Attributes["ID"].Value;
        }
        else
        {
            LeaderTemp = "noapprovers";
        }
        dataFields.Add("CFO", LeaderTemp);

        NodeTemp = xmldoc.SelectSingleNode("//Directors");
        if (NodeTemp != null)
        {
            LeaderTemp = string.IsNullOrEmpty(NodeTemp.Attributes["ID"].Value) ? "noapprovers" : NodeTemp.Attributes["ID"].Value;
        }
        else
        {
            LeaderTemp = "noapprovers";
        }
        dataFields.Add("Directors", LeaderTemp);

        NodeTemp = xmldoc.SelectSingleNode("//VicePresident");
        if (NodeTemp != null)
        {
            LeaderTemp = string.IsNullOrEmpty(NodeTemp.Attributes["ID"].Value) ? "noapprovers" : NodeTemp.Attributes["ID"].Value;
        }
        else
        {
            LeaderTemp = "noapprovers";
        }
        dataFields.Add("VicePresident", LeaderTemp);

        NodeTemp = xmldoc.SelectSingleNode("//AssiPresident");
        if (NodeTemp != null)
        {
            LeaderTemp = string.IsNullOrEmpty(NodeTemp.Attributes["ID"].Value) ? "noapprovers" : NodeTemp.Attributes["ID"].Value;
        }
        else
        {
            LeaderTemp = "noapprovers";
        }
        dataFields.Add("AssiPresident", LeaderTemp);

        NodeTemp = xmldoc.SelectSingleNode("//CounterSignUsers");
        if (NodeTemp != null)
        {
            LeaderTemp = string.IsNullOrEmpty(NodeTemp.Attributes["ID"].Value) ? "" : NodeTemp.Attributes["ID"].Value;
        }
        else
        {
            LeaderTemp = "noapprovers";
        }
        dataFields.Add("CounterSignUsers", LeaderTemp);
        dataFields.Add("IsReport", cbIsReport.Checked ? "1" : "0");
        WorkflowHelper.UpdateDataFields(_BPMContext.Sn, dataFields, DefaultApprover);
    }

    private string SaveWFParams()
    {
        XmlDocument xmldoc = new XmlDocument();
        XmlElement xmleLeaders = xmldoc.CreateElement("Leaders");
        xmldoc.AppendChild(xmleLeaders);
        List<string> ApproverList = new List<string>();
        string LeaderTemp = string.Empty;
        List<string> countersigns = Countersign1.Result.Split(',').ToList();
        string CompanyCode = BPMHelp.GetCompanyCodeByDeptID(StartDeptId);

        if (this.cbChairman.Checked)
        {
            LeaderTemp = GetRoleUsers(CompanyCode, "董事长");
            XmlElement xmleChairman = xmldoc.CreateElement("Chairman");
            xmleLeaders.AppendChild(xmleChairman);
            xmleChairman.SetAttribute("ID", LeaderTemp);
            ApproverList.Add(LeaderTemp);
        }
        if (this.cbPresident.Checked)
        {
            LeaderTemp = GetRoleUsers(CompanyCode, "总裁");
            XmlElement xmlePresident = xmldoc.CreateElement("President");
            xmleLeaders.AppendChild(xmlePresident);
            xmlePresident.SetAttribute("ID", LeaderTemp);
            ApproverList.Add(LeaderTemp);
        }
        if (this.cbEVP.Checked)
        {
            LeaderTemp = GetRoleUsers(CompanyCode, "常务副总裁");
            XmlElement xmleEVP = xmldoc.CreateElement("EVP");
            xmleLeaders.AppendChild(xmleEVP);
            xmleEVP.SetAttribute("ID", LeaderTemp);
            ApproverList.Add(LeaderTemp);
        }
        if (this.cbCFO.Checked)
        {
            LeaderTemp = GetRoleUsers(CompanyCode, "主管财务领导");
            XmlElement xmleCFO = xmldoc.CreateElement("CFO");
            xmleLeaders.AppendChild(xmleCFO);
            xmleCFO.SetAttribute("ID", LeaderTemp);
            ApproverList.Add(LeaderTemp);
        }
        countersigns.Add(StartDeptId);
        if (cbVP.Checked)
        {
            LeaderTemp = string.Empty;
            XmlElement xmlVP = xmldoc.CreateElement("VicePresident");
            xmleLeaders.AppendChild(xmlVP);
            foreach (var item in countersigns)
            {
                string LeaderVPTemp = GetRoleUsers(item, "主管副总裁");
                if (!string.IsNullOrEmpty(LeaderVPTemp) && !ApproverList.Contains(LeaderVPTemp))
                {
                    ApproverList.Add(LeaderVPTemp);
                    LeaderTemp += LeaderVPTemp + ",";
                }
            }
            LeaderTemp = LeaderTemp.TrimEnd(',');
            xmlVP.SetAttribute("ID", LeaderTemp);
        }
        if (cbAP.Checked)
        {
            LeaderTemp = string.Empty;
            XmlElement xmlAP = xmldoc.CreateElement("AssiPresident");
            xmleLeaders.AppendChild(xmlAP);
            foreach (var item in countersigns)
            {
                string LeaderAPTemp = GetRoleUsers(item, "主管助理总裁");
                if (!string.IsNullOrEmpty(LeaderAPTemp) && !ApproverList.Contains(LeaderAPTemp))
                {
                    ApproverList.Add(LeaderAPTemp);
                    LeaderTemp += LeaderAPTemp + ",";
                }
            }
            LeaderTemp = LeaderTemp.TrimEnd(',');
            xmlAP.SetAttribute("ID", LeaderTemp);
        }

        if (cblDirectors.SelectedIndex != -1)
        {
            LeaderTemp = string.Empty;
            XmlElement xmlDirectors = xmldoc.CreateElement("Directors");
            xmleLeaders.AppendChild(xmlDirectors);

            for (int i = 0; i < cblDirectors.Items.Count; i++)
            {
                if (cblDirectors.Items[i].Selected && !ApproverList.Contains(cblDirectors.Items[i].Value))
                {
                    ApproverList.Add(cblDirectors.Items[i].Value);
                    LeaderTemp += cblDirectors.Items[i].Value + ",";
                }
            }
            xmlDirectors.SetAttribute("ID", LeaderTemp);
        }
        countersigns.Remove(StartDeptId);
        if (!string.IsNullOrEmpty(Countersign1.Result))
        {
            LeaderTemp = FilterDataField2(Countersign1.GetCounterSignUsers());
            XmlElement xmleCountersign = xmldoc.CreateElement("CounterSignUsers");
            xmleLeaders.AppendChild(xmleCountersign);
            xmleCountersign.SetAttribute("ID", LeaderTemp);
        }

        if (1 == 1)
        {
            LeaderTemp = GetRoleUsers(CompanyCode, "流程审核人");
            XmlElement xmleWorkflowAuditor = xmldoc.CreateElement("WorkflowAuditor");
            xmleLeaders.AppendChild(xmleWorkflowAuditor);
            xmleWorkflowAuditor.SetAttribute("ID", LeaderTemp);
        }

        if (cbDeptManager.Checked == true)
        {
            LeaderTemp = GetRoleUsers(StartDeptId, "部门负责人");
            XmlElement xmleDeptManager = xmldoc.CreateElement("DeptManager");
            xmleLeaders.AppendChild(xmleDeptManager);
            xmleDeptManager.SetAttribute("ID", LeaderTemp);
        }

        lblApprovers.Text = xmleLeaders.OuterXml;
        return lblApprovers.Text;
    }

    private void SaveData(string ID, string wfStatus)
    {
        InstructionOfEToIInfo obj = null;
        try
        {
            obj = Aitems.Get(ID);
            obj.FormID = ViewState["FormID"].ToString();
            obj.LeadersSelected = lblApprovers.Text;
            obj.IsReport = cbIsReport.Checked ? "1" : "0";

            Aitems.Update(obj);

            WorkFlowInstance workFlowInstance = wf_WorkFlowInstance.GetWorkFlowInstanceByFormId(obj.FormID);
            Countersign1.ProcId = workFlowInstance.InstanceId;
            Countersign1.SaveData(true);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void ChangeResultToUnAgree()
    {
        NameValueCollection dataFields = new NameValueCollection();
        dataFields.Add("IsPass", "0");
        WorkflowHelper.UpdateDataFields(_BPMContext.Sn, dataFields, DefaultApprover);
    }

    void Alert(Page page, object message)
    {
        StringBuilder sb = new StringBuilder();
        //改变鼠标的样式
        string js = string.Format(@"alert('{0}');", message) + sb.ToString();
        ScriptManager.RegisterStartupScript(page, page.GetType(), "ajax", js, true);
    }

    private string GetApproveOpinion(string nodeName)
    {
        string opinion = "";
        List<UserControls_ApproveOpinionUC> options = GetOptions();
        foreach (var item in options)
        {
            if (nodeName == item.CurrentNodeName)
            {
                opinion = ((TextBox)item.FindControl("tbDeptLeaderOpion")).Text;
                return opinion;
            }
        }
        return opinion;
    }

    private List<UserControls_ApproveOpinionUC> GetOptions()
    {
        List<UserControls_ApproveOpinionUC> options = new List<UserControls_ApproveOpinionUC>();
        options.Add(OpinionDeptDiretor);
        options.Add(OpinionDeptManager);
        options.Add(OpinionCountersign);
        options.Add(OpinionAP);
        options.Add(OpinionVP);
        options.Add(OpinionDirectors);
        options.Add(OpinionCFO);
        options.Add(OpinionEVP);
        options.Add(OpinionPresident);
        options.Add(OpinionChairman);
        return options;
    }

    private void InitApproveOpinion(string nodeName)
    {
        TextBox opinion = new TextBox();

        List<UserControls_ApproveOpinionUC> options = GetOptions();
        foreach (var item in options)
        {
            item.InstanceId = _BPMContext.ProcID;
            if (nodeName == item.CurrentNodeName)
            {
                opinion = ((TextBox)item.FindControl("tbDeptLeaderOpion"));
                hf_OpId.Value = opinion.ClientID;
                item.CurrentNode = true;
            }
        }
    }

    private static string GetRoleUsers(string dept, string roleName)
    {
        BFPmsUserRoleDepartment bfurd = new BFPmsUserRoleDepartment();
        StringBuilder dataInfos = new StringBuilder();
        DataTable dtDept = bfurd.GetSelectRoleUser(dept, roleName);
        if (dtDept != null && dtDept.Rows.Count != 0)
        {
            foreach (DataRow rowItem in dtDept.Rows)
            {
                dataInfos.AppendFormat("K2:Founder\\{0},", rowItem["LoginName"].ToString());
            }
        }
        return dataInfos.ToString().Trim(',');
    }
    private string FilterDataField(string dataField_old)
    {
        string dataField = dataField_old.Trim(',');
        if (string.IsNullOrEmpty(dataField))
        {
            dataField = "noapprovers";
        }
        return dataField;
    }
    private string FilterDataField(StringBuilder dataField_old)
    {
        return FilterDataField(dataField_old.ToString().Trim(','));
    }
    private string FilterDataField2(string dataField)
    {
        dataField = dataField.Trim(',');

        if (!dataField.Contains(','))
        {
            if (string.IsNullOrEmpty(dataField))
            {
                dataField = "noapprovers";
            }
        }
        else//多个审批人
        {
            string nowApprovers = "";
            List<string> nowApproverList = dataField.Split(',').ToList();
            foreach (var item in nowApproverList)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    nowApprovers = nowApprovers + item + ",";
                }
            }
            dataField = nowApprovers == "" ? "noapprovers" : nowApprovers.Trim(',');
        }

        return dataField;
    }
    private string FilterDataField2(StringBuilder dataField)
    {
        return FilterDataField2(dataField.ToString().Trim(','));
    }
}