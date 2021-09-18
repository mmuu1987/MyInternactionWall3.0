using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

// Token: 0x02000010 RID: 16
public class TCPUDPSocket : MonoBehaviour
{

  

  

	
	// Token: 0x14000004 RID: 4
	// (add) Token: 0x0600006A RID: 106 RVA: 0x00004174 File Offset: 0x00002374
	// (remove) Token: 0x0600006B RID: 107 RVA: 0x000041AC File Offset: 0x000023AC
	public event Action<string> RecevieDataEvent;

	// Token: 0x0600006C RID: 108 RVA: 0x000041E1 File Offset: 0x000023E1
	private void Awake()
	{
		if (TCPUDPSocket.Instance != null)
		{
			throw new UnityException("单例没有释放");
		}
		TCPUDPSocket.Instance = this;
	}

	// Token: 0x0600006D RID: 109 RVA: 0x00004201 File Offset: 0x00002401
	public void Start_UDPSender(int self_port, int target_port)
	{
		if (this.udp_send_flag)
		{
			return;
		}
		this.udpClient = new UdpClient(self_port);
		this.targetPoint = new IPEndPoint(IPAddress.Parse(GlobalSetting.UDPIP), target_port);
		this.udp_send_flag = true;
	}

	// Token: 0x0600006E RID: 110 RVA: 0x00004238 File Offset: 0x00002438
    private void Start_UDPReceive(int recv_port)
	{
		if (this.udp_recv_flag)
		{
			return;
		}
		
		this.UDPrecv = new UdpClient(new IPEndPoint(IPAddress.Any, recv_port));
		this.endpoint = new IPEndPoint(IPAddress.Any, 0);
		this.recvThread = new Thread(new ThreadStart(this.RecvThread));
		this.recvThread.IsBackground = true;
		this.recvThread.Start();
		this.udp_recv_flag = true;
	}

	// Token: 0x0600006F RID: 111 RVA: 0x000042AA File Offset: 0x000024AA
	public void Close_UDPSender()
	{
		if (!this.udp_send_flag)
		{
			return;
		}
		this.udpClient.Close();
		this.udp_send_flag = false;
	}

	// Token: 0x06000070 RID: 112 RVA: 0x000042C7 File Offset: 0x000024C7
	public void Close_UDPReceive()
	{
		if (!this.udp_recv_flag)
		{
			return;
		}
		this.thrRecv.Interrupt();
		this.thrRecv.Abort();
		this.udp_recv_flag = false;
	}

	// Token: 0x06000071 RID: 113 RVA: 0x000042F0 File Offset: 0x000024F0
	public void Write_UDPSender(string strdata)
	{
		if (!this.udp_send_flag)
		{
			return;
		}
		byte[] bytes = Encoding.Default.GetBytes(strdata);
		int code = this.udpClient.Send(bytes, bytes.Length, this.targetPoint);

		Debug.Log("发送字节数目为：" +code);
	}

    public void Write_UDPSenderHex(string strdata)
    {
        if (!this.udp_send_flag)
        {
            return;
        }
        byte[] bytes =GlobalSetting. HexStringToByteArray(strdata);
        int code = this.udpClient.Send(bytes, bytes.Length, this.targetPoint);

        //Debug.Log("发送字节数目为：" + code);
    }
	// Token: 0x06000072 RID: 114 RVA: 0x00004328 File Offset: 0x00002528
	public string Read_UDPReceive()
	{
		if (this.recvdata == null)
		{
			return null;
		}
		this.returnstr = string.Copy(this.recvdata);
		if (this.old)
		{
			this.old = false;
			this.recvdata = "";
			return this.returnstr;
		}
		return "";
	}

	// Token: 0x06000073 RID: 115 RVA: 0x00004378 File Offset: 0x00002578
	private void ReceiveCallback(IAsyncResult ar)
	{
		this.recvBuf = this.UDPrecv.EndReceive(ar, ref this.endpoint);
		this.recvdata += Encoding.Default.GetString(this.recvBuf);
		this.old = true;
		this.messageReceive = true;
	}

	// Token: 0x06000074 RID: 116 RVA: 0x000043CC File Offset: 0x000025CC
	private void RecvThread()
	{
		this.messageReceive = true;
		for (; ; )
		{
            if (UDPrecv == null) break;

            
            
           // Debug.Log(1);
            
            try
            {
                if (this.messageReceive)
                {
                    
                    this.UDPrecv.BeginReceive(new AsyncCallback(this.ReceiveCallback), null);
                    this.messageReceive = false;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
		}
	}

	// Token: 0x06000075 RID: 117 RVA: 0x00004420 File Offset: 0x00002620
	private void Start()
	{
		
		this.Start_UDPReceive(GlobalSetting.UDPReceivePort);
        Start_UDPSender(GlobalSetting.UDPSelfPort,GlobalSetting.UDPSendPort);
    }

	// Token: 0x06000076 RID: 118 RVA: 0x0000442D File Offset: 0x0000262D
	private void Update()
	{
		this._receiveStr = this.Read_UDPReceive();
		if (!string.IsNullOrEmpty(this._receiveStr) && this.RecevieDataEvent != null)
		{
			this.RecevieDataEvent(this._receiveStr);
		}
	}

    private void OnDestroy()
    {
        CloseUdp();

    }
    public void CloseUdp()
    {
        if (UDPrecv != null)
        {
			Debug.Log("remove this udp");
            UDPrecv.Close();
			
            UDPrecv = null;
            


		}

	}

	// Token: 0x04000061 RID: 97
	private UdpClient udpClient;

	// Token: 0x04000062 RID: 98
	private IPEndPoint targetPoint;

	// Token: 0x04000063 RID: 99
	public static TCPUDPSocket Instance;

	// Token: 0x04000064 RID: 100
	private bool udp_send_flag;

	// Token: 0x04000065 RID: 101
	private bool udp_recv_flag;

	// Token: 0x04000067 RID: 103
	private Thread thrRecv;

	// Token: 0x04000068 RID: 104
	private UdpClient UDPrecv;

	// Token: 0x04000069 RID: 105
	private IPEndPoint endpoint;

	// Token: 0x0400006A RID: 106
	private byte[] recvBuf;

	// Token: 0x0400006B RID: 107
	private Thread recvThread;

	// Token: 0x0400006C RID: 108
	private bool old;

	// Token: 0x0400006D RID: 109
	private string returnstr;

	// Token: 0x0400006E RID: 110
	private string recvdata;

	// Token: 0x0400006F RID: 111
	private bool messageReceive;

	// Token: 0x04000070 RID: 112
	private string _receiveStr;
}
