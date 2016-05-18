var divArray = new Array();         //�����������̽�����
var selectedPosition = -1;          //��ǰ��ѡ��λ��
var intervalHeight = 105;           //���֮�����¼���߶�
var intervalLeft = 500;             //������������
var sign = false;                   //�жϵ���˫���¼�

//�������̽������
var FlowPoint = function(x,y){
    this.xPoint = x;                    //x����
    this.yPoint = y;                    //y����
    this.title = '';                    //��ǰ���Title
    this.enabled = true;                //�Ƿ���ã�Ĭ��Ϊtrue��
    this.selected = false;              //�Ƿ�ѡ��
    this.deleted = true;                //�Ƿ����ɾ��
    this.isView = false;                //�Ƿ��ǲ鿴ģʽ
    this.userId = '';                   //ѡ���ID�˺�
    this.userName = '';                 //�û���
    this.current = false;               //�Ƿ��ǵ�ǰ���(type = current)
    this.counterSigns = new Array();    //�����ǩ������
};

//�����ǩ�����
var CounterSignClass = function(){
    this.counterSignId = '';            //��ǩ��Id
    this.counterSignName = '';          //��ǩ������
    this.enabled = true;                //�Ƿ����
}

//���뵽ĳ��λ��
Array.prototype.InsertAt = function(obj , pos){
    this.splice(pos,0,obj);
};

//���
Array.prototype.clear = function(){
    this.length = 0;
};

//ɾ��ĳ��Ԫ��
Array.prototype.removeAt = function(pos){
    this.splice(pos,1);
};


//��ӽ��
function AddNode(){
    var top = (divArray.length + 1) * intervalHeight;
    var left = intervalLeft;                                    //��߾�
    var tmpFlowPoint = new FlowPoint(left,top);                 //��ӵ����鱣��
    if(selectedPosition == -1){
        divArray.InsertAt(tmpFlowPoint,divArray.length);        //û��Ԫ�ر�ѡ�У�����ӵ����
    }
    else{
        divArray.InsertAt(tmpFlowPoint,selectedPosition + 1);   //��ӵ�ѡ��Ԫ�صĺ���
        
        //���������yPoint��xPointֵ
        for(var i = 0;i < divArray.length;i++){
            divArray[i].yPoint = (i + 1) * intervalHeight;
            divArray[i].xPoint = intervalLeft;
        }
    }
    DisplayFlow();                                              //����div����ʾ
}

//ɾ����Ӧ�Ľ��
function CloseDiv(divObject){
    var ret = confirm('ȷ��ɾ��?');
    if(ret){
        if(divObject.parentNode.parentNode != null){
            for(var i = 0;i < $('.flowNode').length;i++){
                if($('.flowNode')[i].id === divObject.parentNode.parentNode.id){
                    divArray.removeAt(i);                           //�������ɾ����ӦԪ��
                    
                    //����Ԫ����ʾλ��
                    for(var j = 0;j < divArray.length;j++){
                        divArray[j].yPoint = (j + 1) * intervalHeight;
                        divArray[j].xPoint = intervalLeft;
                    }
                    break;
                }
            }
            DisplayFlow();                                          //��������ͼ
        }
    }
}

//ɾ����ǩ���
function CloseSignNode(divId,delId,delName){
    var ret = confirm('ȷ��ɾ��?');
    if(ret){
        if(divId != null){
            for(var i = 0;i < $('.flowNode').length;i++){
                if($('.flowNode')[i].id == divId){
                    for(var j = 0;j < divArray[i].counterSigns.length;j++){
                        if(divArray[i].counterSigns[j].counterSignId == delId && divArray[i].counterSigns[j].enabled)
                            divArray[i].counterSigns.removeAt(j);
                    }
                    break;
                }
            }
            DisplayFlow();                  //���»�������ͼ
        }
    }
}

//��ӻ�ǩ��
function counterSign(imgObject){
    var returnVal = window.showModalDialog('../CommonPage/SelectPersonnel.aspx',null,'dialogHeight:375px;dialogWidth:575px;scroll:0');
    if(returnVal == null || returnVal.length == 0){
        return;
    }
    else if(returnVal[0] == "" && returnVal[1] == ""){
        return;
    }
    else{
        if(imgObject.parentNode.parentNode != null){
            for(var i = 0;i < $('.flowNode').length;i++){
                if($('.flowNode')[i].id === imgObject.parentNode.parentNode.id){
                    var tmpIds = returnVal[0].split(';');
                    var tmpNames = returnVal[1].split(';');
                    var tmpPNames = returnVal[2].split(';');
                    var tmpdeptNames = returnVal[3].split(';');
                    var isOk = true;                            //��ʾ�Ƿ�������
                    for(var j = 0;j < tmpIds.length;j++){
                        for(var k = 0;k < divArray[i].counterSigns.length;k++){
                            if(divArray[i].counterSigns[k].enabled && divArray[i].counterSigns[k].counterSignId == tmpIds[j]){
                                isOk = false;                   //����ѡ�����к����Ѵ��ڵģ������
                            }
                        }
                        if(isOk){
                            var counterSign = new CounterSignClass();
                            counterSign.counterSignId = tmpIds[j];
                            counterSign.counterSignName = '&nbsp;&nbsp;&nbsp;������' + tmpNames[j] + '(' + tmpPNames[j] + ')' + '\n&nbsp&nbsp&nbsp���ţ�' + tmpdeptNames[j];
                            counterSign.enabled = true;
                            divArray[i].counterSigns.push(counterSign);
                        }
                    }
                }
            }
            DisplayFlow();              //���²�������ͼ
        }
    }
}

//���������е�������ʾ����ͼ
function DisplayFlow(){
    var Area = document.getElementById('InteractiveArea');      //ȡ�û����
    Area.innerHTML = '';
    var maxWidth = 0;              //��ʾ�����
    for(var i = 0;i < divArray.length;i++){
        if(divArray[i].userName == null)
            divArray[i].userName = '';
        //���δ�ߵ��ý�� (��ѡ�С���ɾ�������޸�)
        if(divArray[i].enabled && !divArray[i].current){
            if(divArray[i].selected && !divArray[i].isView){           //��ѡ�в��Ҳ��ǲ鿴ģʽ
                Area.innerHTML += '<div id="divPanel'+ i +'" class="flowNode" style="position:absolute; top:'+ divArray[i].yPoint +'px; left:'+ divArray[i].xPoint +'px; border-right: #ff0000 1px dashed; border-top: #ff0000 1px dashed; border-left: #ff0000 1px dashed; border-bottom: #ff0000 1px dashed; background-image:url()"><div id="divTitle'+ i +'" class="flowTitle"><span style="float:left; padding-top:7px; padding-left:5px;">'+ divArray[i].title +'</span><img src="../Images/close.gif" style="float:right; padding-right:3px; padding-top:8px;" alt="ɾ�����" onclick="CloseDiv(this);" /></div><div style="width:100%; height:68px;" onclick="SelectDiv(this);" ondblclick="SelectUser(this);"><span class="nodeName" ondblclick="SelectUser(this.parentNode);">'+ divArray[i].userName.replace(/\n/g,"<br/>") +'</span></div></div>';
            }
            else if(!divArray[i].selected && !divArray[i].isView){      //δ��ѡ�в��Ҳ��ǲ鿴ģʽ
                Area.innerHTML += '<div id="divPanel'+ i +'" class="flowNode" style="position:absolute; top:'+ divArray[i].yPoint +'px; left:'+ divArray[i].xPoint +'px;"><div id="divTitle'+ i +'" class="flowTitle"><span style="float:left; padding-top:7px; padding-left:5px;">'+ divArray[i].title +'</span><img src="../Images/close.gif" style="float:right; padding-right:3px; padding-top:8px;" alt="ɾ�����" onclick="CloseDiv(this);" /></div><div style="width:100%; height:68px;" onclick="SelectDiv(this);" ondblclick="SelectUser(this);"><span class="nodeName">'+ divArray[i].userName.replace(/\n/g,"<br/>") +'</span></div></div>';
            }
            else{
                Area.innerHTML += '<div id="divPanel'+ i +'" class="flowNode" style="position:absolute; top:'+ divArray[i].yPoint +'px; left:'+ divArray[i].xPoint +'px;"><div id="divTitle'+ i +'" class="flowTitle"><span style="float:left; padding-top:7px; padding-left:5px;">'+ divArray[i].title +'</span></div><div style="width:100%; height:100%;"><span class="nodeName">'+ divArray[i].userName.replace(/\n/g,"<br/>") +'</span></div></div>';
            }
        }
        //����ý���ǵ�ǰ�����ߵĽ�㣨��ѡ�С�����ɾ���������޸ġ��ɻ�ǩ��
        else if(divArray[i].enabled && divArray[i].current){
            if(divArray[i].selected && !divArray[i].isView){           //��ѡ��
                Area.innerHTML += '<div id="divPanel'+ i +'" class="flowNode" style="position:absolute; top:'+ divArray[i].yPoint +'px; left:'+ divArray[i].xPoint +'px; border-right: #ff0000 1px dashed; border-top: #ff0000 1px dashed; border-left: #ff0000 1px dashed; border-bottom: #ff0000 1px dashed; background-image:url()"><div id="divTitle'+ i +'" class="flowTitle"><span style="float:left; padding-top:7px; padding-left:5px;">'+ divArray[i].title +'</span><img src="../Images/additem.gif" style="float:right; padding-right:3px; padding-top:8px;" alt="��ǩ" onclick="counterSign(this);" /></div><div style="width:100%; height:100%;" onclick="SelectDiv(this);"><span class="nodeName">'+ divArray[i].userName.replace(/\n/g,"<br/>") +'</span></div></div>';
            }
            else if(!divArray[i].selected && !divArray[i].isView){
                Area.innerHTML += '<div id="divPanel'+ i +'" class="flowNode" style="position:absolute; top:'+ divArray[i].yPoint +'px; left:'+ divArray[i].xPoint +'px;"><div id="divTitle'+ i +'" class="flowTitle"><span style="float:left; padding-top:7px; padding-left:5px;">'+ divArray[i].title +'</span><img src="../Images/additem.gif" style="float:right; padding-right:3px; padding-top:8px;" alt="��ǩ" onclick="counterSign(this);" /></div><div style="width:100%; height:100%;" onclick="SelectDiv(this);"><span class="nodeName">'+ divArray[i].userName.replace(/\n/g,"<br/>") +'</span></div></div>';
            }
            else{
                Area.innerHTML += '<div id="divPanel'+ i +'" class="flowNode" style="position:absolute; top:'+ divArray[i].yPoint +'px; left:'+ divArray[i].xPoint +'px;"><div id="divTitle'+ i +'" class="flowTitle"><span style="float:left; padding-top:7px; padding-left:5px;">'+ divArray[i].title +'</span></div><div style="width:100%; height:100%;"><span class="nodeName">'+ divArray[i].userName.replace(/\n/g,"<br/>") +'</span></div></div>';
            }
        }
        //����ý���Ѿ��߹�������ѡ�С�����ɾ���������޸ģ�
        else if(!divArray[i].enabled){
            Area.innerHTML += '<div id="divPanel'+ i +'" class="flowNode" style="position:absolute; top:'+ divArray[i].yPoint +'px; left:'+ divArray[i].xPoint +'px;"><div id="divTitle'+ i +'" class="unavailableFlowTitle"><span style="float:left; padding-top:7px; padding-left:5px;">'+ divArray[i].title +'</span></div><div style="width:100%; height:100%;"><span class="nodeName">'+ divArray[i].userName.replace(/\n/g,"<br/>") +'</span></div></div>';
        }
        
        //������ǩ��
        if(divArray[i].counterSigns.length != 0){                 //��ǩ
            for(var k = 0;k < divArray[i].counterSigns.length;k++){
                var counterSignLeft = (divArray[i].xPoint + 230) + 160 * k;      //��ǩ��߾�
                if(maxWidth < counterSignLeft)
                    maxWidth = counterSignLeft;
                var counterSignTop = divArray[i].yPoint + 15;                         //��ǩ�߶�
                if(divArray[i].counterSigns[k].enabled){
                    Area.innerHTML += '<div class="counterSignNode" style="position:absolute; left:'+ counterSignLeft +'px; top:'+ counterSignTop +'px;"><div class="counterSignTitle"><img alt="ɾ��" src="../Images/close.gif" onclick="CloseSignNode(\'divPanel'+ i +'\','+ divArray[i].counterSigns[k].counterSignId +',\'' + divArray[i].counterSigns[k].counterSignName.replace(/\n/g,"<br/>") + '\');" style="float:right; padding-right:3px; padding-top:2px;" /></div><div ondblclick="EditSignNode(\'divPanel'+ i +'\','+ divArray[i].counterSigns[k].counterSignId +',\'' + divArray[i].counterSigns[k].counterSignName.replace(/\n/g,"<br/>") + '\');">'+ divArray[i].counterSigns[k].counterSignName.replace(/\n/g,"<br/>") +'</div></div>';
                }
                else{
                    Area.innerHTML += '<div class="counterSignNode" style="position:absolute; left:'+ counterSignLeft +'px; top:'+ counterSignTop +'px; background-color:Gray;"><div class="counterSignTitle"></div><div>'+ divArray[i].counterSigns[k].counterSignName.replace(/\n/g,"<br/>") +'</div></div>';
                }
            }
        }
    }
    
    if(divArray != null && divArray.length != 0)
    {
        //����div�߶�
        var divHeight = divArray[divArray.length - 1].yPoint + 50;
        $('#InteractiveArea').css('height',divHeight);
        if(maxWidth != 0){
            $($('#divFlow')[0].children[0]).css('width',((divArray[0].xPoint + maxWidth + 400)/2));
            $('#divFlow').css('width',((divArray[0].xPoint + maxWidth + 400)/2));
        }
    }
}

//ѡ����
function SelectUser(divObject){
    sign = true;
    var returnVal = window.showModalDialog('../CommonPage/SelectPersonnel.aspx',null,'dialogHeight:375px;dialogWidth:575px;scroll:0');
    if(returnVal == null || returnVal.length == 0){
        return;
    }
    else if(returnVal[0] == "" && returnVal[1] == ""){
        return;
    }
    else{
        if(divObject.parentNode != null){
            for(var i = 0;i < $('.flowNode').length;i++){
                if($('.flowNode')[i].id === divObject.parentNode.id){
                    divArray[i].userId = returnVal[0].split(';')[0];
                    divArray[i].userName = '\n&nbsp;&nbsp;&nbsp;������' + returnVal[1].split(';')[0] + '(' + returnVal[2].split(';')[0] + ')' + '\n--------------------------------------------------\n' + '&nbsp;&nbsp;&nbsp;���ţ�' + returnVal[3].split(';')[0];
                }
            }
            
            DisplayFlow();              //���²�������ͼ
        }
    }
}

//�༭��ǩ���
function EditSignNode(divId,userId,userName){
    var returnVal = window.showModalDialog('../CommonPage/SelectPersonnel.aspx',null,'dialogHeight:375px;dialogWidth:575px;scroll:0');
    if(returnVal == null || returnVal.length == 0){
        return;
    }
    else if(returnVal[0] == "" && returnVal[1] == ""){
        return;
    }
    else{
        for(var i = 0;i < $('.flowNode').length;i++){
            if($('.flowNode')[i].id == divId){              //���Ҷ�Ӧ��divArray��index
                for(var j = 0;j < divArray[i].counterSigns.length;j++){
                    if(divArray[i].counterSigns[j].enabled && divArray[i].counterSigns[j].counterSignId == userId){
                        divArray[i].counterSigns[j].counterSignId = returnVal[0].split(';')[0];
                        divArray[i].counterSigns[j].counterSignName = '&nbsp;&nbsp;&nbsp;������' + returnVal[1].split(';')[0] + '(' + returnVal[2].split(';')[0] + ')' + '\n&nbsp;&nbsp;&nbsp;���ţ�' + returnVal[3].split(';')[0];
                        break;
                    }
                }
            }
        }
        DisplayFlow();
    }
}

//��ѡĳ�����
function SelectDiv(divObject){
    setTimeout(function(){
        if(sign){
            sign = false;
            return;
        }
        if(divObject != null){
            for(var i = 0; i<divArray.length; i++){
                if(divObject.parentNode != null && divObject.parentNode.id != null && divObject.parentNode.id == ('divPanel' + i)){
                    if(divArray[i].enabled && !divArray[i].selected){
                        divArray[i].selected = true;
                        selectedPosition = i;
                    }
                    else{
                        divArray[i].selected = false;
                        selectedPosition = -1;
                    }
                }
                else if(divArray[i].enabled){
                    divArray[i].selected = false;
                }
            }DisplayFlow();
        }
    },200);
}

//��֤����ͼ��Ϣ�Ƿ���д����
function VerificationFlowIntegrity(){
    if(divArray != null && divArray.length != 0){
        for(var i = 0;i < divArray.length;i++){
            if(divArray[i].userId == '' || divArray[i].userName == '' || divArray[i].userName == 'δ�ҵ��û���Ϣ'){
                return false;
            }
            if(divArray[i].counterSigns != null && divArray[i].counterSigns.length != 0){       //����ǩ��
                for(var j = 0;j < divArray[i].counterSigns.length;j++){
                    if(divArray[i].counterSigns[j].counterSignId == '' || divArray[i].counterSigns[j].counterSignName == ''){
                        return false;
                    }
                }
            }
        }
        return true;
    }
    else{
        return false;
    }
}

$(document).ready(function(){
    ResumeFlowInfo();
});