K2.Office.MyapplicantPanel = function(config) {
	K2.Office.MyapplicantPanel.superclass.constructor.call(this, config);

//	var proxy = new Ext.data.HttpProxy( {
//		url : 'myapplicant.aspx'
//	});

//	var recordType = new Ext.data.Record.create([ {
//		name : "id",
//		type : "int"
//	}, {
//		name : "comNum",
//		type : "string"
//	}, {
//		name : "comName",
//		type : "string"
//	}, {
//		name : "comAddress",
//		type : "string"
//	}]);

//	// ���������
//	var reader = new Ext.data.JsonReader( {
//		totalProperty : "results",
//		root : "rows",
//		id : "id"
//	}, recordType);

//	// ����store
//	var ds = new Ext.data.Store( {
//		proxy : proxy,
//		reader : reader
//	});
//    this.ds=ds;
//	// �ڶ�����һ��cm,grid

//var sm= new Ext.grid.CheckboxSelectionModel();
//	var cm = new Ext.grid.ColumnModel( {
//		defaultSortable : true,
//		defaultWidth : 180,
//		columns : [ sm,{
//			header : '���',
//			dataIndex : 'comNum'
//		}, {
//			header : '����',
//			dataIndex : 'comName'
//		}, {
//			header : '��˾��ַ',
//			width : 300,
//			dataIndex : 'comAddress'
//		}]
//	});
//var pagingBar = new Ext.PagingToolbar({
//        pageSize: 10,
//        store: ds,
//        displayInfo: true,
//        displayMsg: '���� {2}����ǰ��ʾ {0} - {1}��',
//        emptyMsg: "û������"
//    });
//    
//	var grid = new Ext.grid.GridPanel( {
//		cm : cm,
//		sm:sm,
//		stripeRows :true,
//		store : ds,
//		width : 660,
//		height : 400,
//		bbar:pagingBar,
//		loadMask:{msg:'������������,���Ե�...'},
//		title : '��˾�б�'
//	});
//	//ds.load();

//	this.add(grid);
	//	// ������������tbar��ҳ,������
	var panel = new Ext.Panel({
	    defaults: {
	        autoWidth: true,
	        autoScroll: true,
	        autoHeight: true
	    },
	    title: '�ҵ�����',
	    html: '<iframe id="iMyApplicant" src="../WorkSpace/MyStarted.aspx" width="100%" frameborder="0" height="370"></iframe>'
	});

	this.add(panel);
	
}
Ext.extend(K2.Office.MyapplicantPanel, Ext.Panel, {});