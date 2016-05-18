Ext.ns("K2", "K2.Office", "K2.Util", "K2.Office.Department");
/*Ext.apply(obj, config, [defaults]) ��config������������Զ����Ƶ���һ������obj�ϣ� 
����������defaults���������ṩĬ��ֵ�� ����ͨ��ָ��ǰ���������͹��ˡ� 
���������Ҫ���ڹ��캯���У� ���������ø��Ƶ�������*/
K2.Office.LeftMenu = function(config) {
	var d = Ext.apply( {// default set
				width : 200,
				split : true,
				region : 'west',
				defaults : {
					border : false
				},
				layoutConfig : {
					animate : true
				}
			}, config || {});

	config = Ext.apply(d, {
		layout : 'accordion',
		collapsible : true
	});
	
	K2.Office.LeftMenu.superclass.constructor.call(this, config);
	
	//�Ľ�����Ϊ�����˸�������tree!
	for(var i=0;i<this.trees.length;i++)		 
	 	this.add({title:this.trees[i].getRootNode().text,items:[this.trees[i]]});	

	// �¼�����
	this.addEvents('nodeClick');
	this.initTreeEvent();
	}

Ext.extend(K2.Office.LeftMenu, Ext.Panel, {
	initTreeEvent : function() {
		if(!this.items) return;
		for (var i = 0;i < this.items.length; i++) {
			var p = this.items.itemAt(i);
			if (p)
			var t = p.items.itemAt(0);
			if(t)
			t.on( {
				'click' : function(node, event) {
					if (node && node.isLeaf()) {
						event.stopEvent();
						this.fireEvent('nodeClick', node.attributes);
					}
				},
				scope : this
			});
		}
	}
})
