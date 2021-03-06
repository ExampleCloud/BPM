USE [SohuK2FrameWork]
GO
/****** Object:  StoredProcedure [dbo].[P_K2_GetProcessCurrentXMLValue]    Script Date: 05/16/2012 14:20:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

ALTER proc [dbo].[P_K2_GetProcessCurrentXMLValue]
@SrcProcInstID int
as
SET NOCOUNT ON;
	------=============获取主流程及子流程的ID================-------------begin
    declare @t table(DstProcInstID int)
    insert @t select DstProcInstID from K2ServerLog.dbo._IPC IPC 
    inner join K2ServerLog.dbo._ProcInst ProcInst on ProcInst.ID=IPC.DstProcInstID
    where IPC.SrcProcInstID = @SrcProcInstID and ProcInst.Status in (1,2)
    while @@rowcount > 0
        insert @t select a.DstProcInstID from K2ServerLog.dbo._IPC as a 
        inner join @t as b on a.SrcProcInstID = b.DstProcInstID and a.DstProcInstID not in(select DstProcInstID from @t)
		inner join K2ServerLog.dbo._ProcInst ProcInst on ProcInst.ID=a.DstProcInstID and ProcInst.Status in (1,2)
   
   Declare @IDS nvarchar(500)   
   set @IDS=@SrcProcInstID
   Select @IDS=coalesce(@IDS,'')+'_'+ Convert(nvarchar(50),DstProcInstID) from @t order by DstProcInstID   
   --select @IDS as IDS
	------=============获取主流程及子流程的ID================-------------end
   
    --主审批链
	declare @xmlProcessApprovalChain xml='';
	
	 --子流程中的XML
	declare @xmlSubApprovalChain xml='';	

	declare @Position int=1;
	declare @id nvarchar(4000);
	set @id=dbo.Split(@IDS,'_',@Position);	
	
	while @id<>''
	begin			
		--流程类型
		declare @procname nvarchar(100)='';
		
		SELECT  @procname=[procset].name
		  FROM [K2ServerLog].[dbo].[_ProcInst] [procinstid]
		  inner join [K2ServerLog].[dbo].[_Proc] [proc] on  [proc].id=[procinstid].procid 
		  inner join [K2ServerLog].[dbo].[_ProcSet] [procset] on  [procset].id=[proc].procsetid   
		  where [procinstid].id=@id	  
		  --select @procname;
		  	
		if  @procname='SCF'	
		begin	
			--获取主审批链
			SELECT @xmlProcessApprovalChain=[Value]
			FROM [K2ServerLog].[dbo].[_ProcInstXml] where procinstid=@id and name='ProcessApprovalChain'
		end
		else if @procname='Controller'
		begin
			SELECT @xmlSubApprovalChain=[Value]
			FROM [K2ServerLog].[dbo].[_ProcInstData] where procinstid=@id and name='SubApprovalChain'			
		end
		else if @procname='Approve'
		begin
			SELECT @xmlSubApprovalChain=[Value]
			FROM [K2ServerLog].[dbo].[_ProcInstData] where procinstid=@id and name='Destinations'
		end	
		
		declare @CurrentID nvarchar(36)=@xmlSubApprovalChain.value('(/*/ID)[1]', 'nvarchar(36)');
		
		declare @modifySql nvarchar(max) = ' set @XmlContentPar.modify(''insert '+cast(@xmlSubApprovalChain as nvarchar(max))+' after (//*[ID="'+@CurrentID+'"])[1]'');'
										 + ' set @XmlContentPar.modify(''delete (//*[ID="'+@CurrentID+'"])[1]'')'
										 + ' select @XmlContentParOut=@XmlContentPar;'
		exec sp_executesql @modifySql, N'@XmlContentParOut xml output,@XmlContentPar as xml',@xmlProcessApprovalChain output,@XmlContentPar=@xmlProcessApprovalChain
		
		
		set @Position=@Position+1;
		set @id=dbo.Split(@IDS,'_',@Position);	
	end
	
	select @xmlProcessApprovalChain ProcessApprovalChain
	
	--select @xmlProcessApprovalChain.query('(//*[ID="3c63928c-1d24-48e9-91a7-44c8c6fab2b2"])')

