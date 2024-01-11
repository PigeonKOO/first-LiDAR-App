using System;
using System.IO.Ports;
using System.Threading;

class Program
{
    static bool lidarRunning = false;

    static void Main()
    {
        string lidarPort = "COM6";  // RPLIDAR가 연결된 COM 포트 지정
        int baudRate = 115200;      // RPLIDAR와 통신할 Baud Rate 지정

        Console.WriteLine("'start' to begin LiDAR, 'stop' to stop.");
        string command = Console.ReadLine();

        if (command.ToLower() == "start")
        {
            StartLidar(lidarPort, baudRate);


            Console.WriteLine("LiDAR started. Type 'stop' to stop.");


            while (Console.ReadLine().ToLower() != "stop") ;

            // 프로그램이 종료될 때 LiDAR 정지
            StopLidar();
        }
        else
        {
            Console.WriteLine("command. Type 'start' to begin LiDAR.");
        }
    }

    static void StartLidar(string portName, int baudRate)
    {
        // 라이다 연결
        RplidarBinding.OnConnect(portName);

        // 모터 시작
        RplidarBinding.StartMotor();

        // 스캔 시작
        RplidarBinding.StartScan();

        // 라이다 동작 설정
        lidarRunning = true;

        // 라이다 데이터 수신 스레드 시작
        Thread lidarThread = new Thread(ReceiveAndPrintData);
        lidarThread.Start();
    }

    static void StopLidar()
    {
        // 라이다 동작  해제
        lidarRunning = false;

        // 스레드가 종료될 때까지 대기
        Thread.Sleep(1000);

        // 라이다 스캔 중지
        RplidarBinding.EndScan();

        // 모터 정지
        RplidarBinding.EndMotor();

        // 라이다 연결 해제
        RplidarBinding.OnDisconnect();
    }

    static void ReceiveAndPrintData()
    {
        LidarData[] data = new LidarData[720];

        while (lidarRunning)
        {
         
            var datalen = RplidarBinding.GetData(ref data);

            for (var i = 0; i < datalen; i++)
            {
                Console.WriteLine($"{data[i].distant} {data[i].theta}");
            }

        }
    }
}