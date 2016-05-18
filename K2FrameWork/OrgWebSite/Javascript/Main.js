Ext.onReady(function() {

    Ext.BLANK_IMAGE_URL = '../pic/s.gif';
    Ext.QuickTips.init();
    Ext.lib.Ajax.defaultPostHeader += ";charset=utf-8";

    // 1������head����
    var head = new Ext.Panel({
        region: 'north',
        border: false,
        html: '<table cellspacing="0" cellpadding="0" border="0" width="100%"><tr><td valign="top" align="left" background="../pic/top-bg.jpg"><img height="98" width="302" src="../pic/portal-logo.jpg"></td></tr></table>',
        height: 98
    });

    // 2������foot����
    var foot = new Ext.Panel({
        region: 'south',
        html: '<div style="background-color:#F2F2F2; height:30px; text-align:right"><img src="../pic/bottom.jpg" width="232" height="25" />'
					+ '</div>',
        height: 35
    });

    // 3������leftMenu����
    // var leftmenu = new Ext.Panel( {
    // region : 'west',
    // html : '<div>�����˵�</div>',
    // width : 200
    // });

    // 4�����������ݲ���
    // var mainTab = new Ext.Panel( {
    // region : 'center',
    // html : '<div>�����ݲ���</div>'
    // });

    var t1 = new Ext.tree.TreePanel({
        border: false,
        rootVisible: false,
        root: new Ext.tree.AsyncTreeNode({
            text: "������ƽ̨",
            expanded: true,
            children: [{
                id: "sendFlow",
                text: "��������",
                leaf: true
            }, {
                id: "myApplicant",
                text: "�ҵ�����",
                leaf: true
            }, {
                id: "myWait",
                text: "�ҵĴ���",
                leaf: true
            }, {
                id: "myHandled",
                text: "�ҵ��Ѱ�",
                leaf: true
            }, {
                id: 'myDraf',
                text: '�ݸ���',
                leaf: true
            }
				]
        })
    });

    var t2 = new Ext.tree.TreePanel({
        border: false,
        rootVisible: false,
        root: new Ext.tree.AsyncTreeNode({
            text: "ϵͳ����",
            expanded: true,
            children: [{
                id: "orgMNG",
                text: "��֯�ṹ����",
                leaf: true
            }, {
                id: "visualOrgMNG",
                text: "������֯����",
                leaf: true
            }, {
                id: 'flowRulesMNG',
                text: '���̹������',
                leaf: true
            }, {
                id: 'dicMNG',
                text: '�����ֵ����',
                leaf: true
            }, {
                id: 'flowMNG',
                text: '���̹���',
                leaf: true
            }, {
                id: "permissions",
                text: "Ȩ�޹���",
                leaf: true
                //					children : [ {
                //						id : "permission",
                //						text : "Ȩ�޹���",
                //						leaf : true
                //					}, {
                //						id : "permissionType",
                //						text : "Ȩ�����",
                //						leaf : true
                //					}]
}]
            })

        });

        //�Զ���������
        var t3 = new Ext.tree.TreePanel({
            border: false,
            rootVisible: false,
            root: new Ext.tree.AsyncTreeNode({
                text: "�Զ�������",
                expanded: true,
                children: [{
                    id: "appTableMNG",
                    text: "���ݱ����",
                    leaf: true
                },
                {
                    id: "customizeFormMNG",
                    text: "��ģ�����",
                    leaf: true
                },
                {
                    id: "policyMNG",
                    text: "���̹������",
                    leaf: true
                }
                ]
        })
    });

    var leftmenu = new K2.Office.LeftMenu({
        title: 'K2BPM',
        trees: [t1, t2, t3]
    });

    var mainTab = new K2.Office.MainingPanel({
        style: 'padding:0 6px 0 0',
        autoScroll: true,
        region: 'center',
        deferredRender: false,
        activeTab: 0,
        resizeTabs: true,
        inTabWidth: 100,
        tabWidth: 110,
        enableTabScroll: true
    });

    // 5������leftmenu��mainTab����֮��Ĺ�ϵ
    leftmenu.on("nodeClick", function(nodeAttr) {
        mainTab.loadTab(nodeAttr);
    });
    // 6����������
    var viewport = new Ext.Viewport({
        layout: 'border',
        style: 'border:#024459 2px solid;',
        items: [head, foot, leftmenu, mainTab]
    });
});