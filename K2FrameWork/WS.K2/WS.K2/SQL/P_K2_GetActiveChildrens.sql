USE [K2FrameWorkCMC]
GO
/****** Object:  StoredProcedure [dbo].[sp_Recursion_GetActiveChildrens]    Script Date: 04/05/2012 10:32:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

ALTER proc [dbo].[P_K2_GetActiveChildrens]
@SrcProcInstID int
as
SET NOCOUNT ON;
    declare @t table(DstProcInstID int)
    insert @t select DstProcInstID from K2ServerLog.dbo._IPC IPC 
    inner join K2ServerLog.dbo._ProcInst ProcInst on ProcInst.ID=IPC.DstProcInstID
    where IPC.SrcProcInstID = @SrcProcInstID and ProcInst.Status in (1,2)
    while @@rowcount > 0
        insert @t select a.DstProcInstID from K2ServerLog.dbo._IPC as a 
        inner join @t as b on a.SrcProcInstID = b.DstProcInstID and a.DstProcInstID not in(select DstProcInstID from @t)
		inner join K2ServerLog.dbo._ProcInst ProcInst on ProcInst.ID=a.DstProcInstID and ProcInst.Status in (1,2)
   
   Declare @IDS nvarchar(100)   
   set @IDS=@SrcProcInstID
   Select @IDS=coalesce(@IDS,'')+'_'+ Convert(nvarchar(50),DstProcInstID) from @t order by DstProcInstID   
   select @IDS as IDS



