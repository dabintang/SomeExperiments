<template>
  <div class="hello">
    【无验权定时广播测试】
    <br/>
    服务器时间：<span>{{ serviceTime1Format }}</span>
    <br/><hr>
    
    【无验权发送消息测试】
    <br/>
    <input type="text" v-model="text" />
    <button @click="sendToAll">发送给所有人</button>
    <button @click="sendToSelf">发送给自己</button>
    <button @click="sendToOthers">发送给其他人</button>
    <br/>
    接收到的消息：<span>{{ msg }}</span>
    <br/><hr>

    【带身份认证发送消息测试】
    <br/>
    验权连接：<div><button @click="connOnAuth" :disabled="connAuth!=null">连接</button>token：<input type="text" v-model="authToken" width="300px"/></div>
    <br/>
    <span>{{ authConnMsg }}</span>
    <br/>
    <button @click="sendToUID">给指定uid发送消息</button><span>uid：</span><input type="text" v-model="uid" width="100px"/>消息：<input type="text" v-model="uidText" />
    <br/>
    <span>{{ authMsg }}</span>
  </div>
</template>

<script>
import moment from "moment";
const signalR = require("@microsoft/signalr");
export default {
  data() {
    return {
      connection: null,  //signalr连接
      serviceTime: null, //服务器时间
      connMsg: null,  //signalr连接
      text: '', //输入框内容
      msg: '', //接收到的消息
      connAuth: null,  //signalr连接
      authToken: '', //token
      authConnMsg: '', //带权限连接情况
      authMsg: '', //带权限连接消息
      uid: '', //uid
      uidText: '' //uid
    }
  },
  created() {
    console.log("created");
    // 初始化
    this.init();
  },
  computed: {
    serviceTime1Format() {
      if(this.serviceTime) {
        return moment(this.serviceTime).format('yyyy-MM-DD HH:mm:ss');
      } else {
        return '';
      }
    }
  },
  methods:{
    //初始化
    init() {
        this.connection = new signalR.HubConnectionBuilder()
          .withUrl("http://localhost:36921/ServerTimeHub", { })
          .configureLogging(signalR.LogLevel.Debug)
          .build();
        this.connMsg = new signalR.HubConnectionBuilder()
          .withUrl("http://localhost:36921/SendMsgHub", { })
          .configureLogging(signalR.LogLevel.Information)
          .build();
 
      // 客户端注册事件，注册事件回调参数个数应与后端一致
      this.connection.on("BroadcastTime", data => {
        // console.log("BroadcastTime"+data);
        this.serviceTime = new Date(data);
      });
      this.connMsg.on("ReceiveMessage", data => {
        // console.log("ReceiveMessage"+data);
        this.msg = data;
      });
  
      // 生命周期
      this.connection.onreconnecting(error => {
        console.log("onreconnecting："+error);
      });
      this.connection.onreconnected(connectionId => {
        console.log("onreconnected："+connectionId);
        this.sendMsg();
      });
      this.connection.onclose(error => {
        console.log("onclose："+error);
      });

      //启动
      this.connection.start();
      this.connMsg.start();
    },
    //发送信息给所有人
    sendToAll() {
      this.connMsg.invoke("SendToAll", this.text);
    },
    //发送信息给自己
    sendToSelf() {
      this.connMsg.invoke("SendToSelf", this.text);
    },
    //发送信息给其他人
    sendToOthers() {
      this.connMsg.invoke("SendToOthers", this.text);
    },
    //带身份认证连接
    connOnAuth() {
      this.connAuth = new signalR.HubConnectionBuilder()
          .withUrl("http://localhost:36921/AuthHub", { accessTokenFactory: () => this.authToken })
          .configureLogging(signalR.LogLevel.Trace)
          .build();

      this.connAuth.on("ReceiveMessage", data => {
        // console.log("ReceiveMessage"+data);
        if (!this.authConnMsg) {
          this.authConnMsg = data;
        }
        this.authMsg = data;
      });

      this.connAuth.start();
    },
    //给指定uid发送消息
    sendToUID() {
      this.connAuth.invoke("SendToUID", this.uid, this.uidText);
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
</style>
