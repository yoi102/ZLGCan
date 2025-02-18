﻿using ZLGCanComm.Enums;
using ZLGCanComm.Structs;

namespace ZLGCanComm.Devices;

public class UsbCan1Device : BaseDevice
{
    private InitConfig initConfig;
    private bool isOpened;

    public UsbCan1Device() : base(0)
    {
        initConfig = new InitConfig()
        {
            AcceptanceCode = 0x00000000, //验收码
            AcceptanceMask = 0xFFFFFFFF, //验收屏蔽码
            Filter = 1,          //滤波方式 1: 单滤波 0: 双滤波
            Mode = 0,          //0:正常模式 1:只听模式

            Timing0 = 0x01,       //通讯速率  250Kbps
            Timing1 = 0x1C,
        };
    }

    public UsbCan1Device(uint canIndex, InitConfig initConfig) : base(canIndex)
    {
        this.initConfig = initConfig;
    }

    public override uint UintDeviceType => (uint)DeviceType.VCI_USBCAN1;

    /// <summary>
    /// 尝试连接设备，如果连接上将返回True，否则返回 false
    /// </summary>
    /// <returns></returns>
    public override void Connect()
    {
        if (disposed)
            throw new InvalidOperationException();
        if (IsConnected)
            return;

        if (!isOpened)
        {
            var device_index = DeviceRegistry.GetUniqueDeviceIndex(DeviceType);
            deviceIndex = device_index;
        }
        else
        {
            ZLGApi.VCI_CloseDevice(UintDeviceType, deviceIndex);
        }

        if (ZLGApi.VCI_OpenDevice(UintDeviceType, deviceIndex, 0) == (uint)OperationStatus.Failure)
            throw new CanDeviceOperationException();
        isOpened = true;
        var config = StructConverter.Converter(initConfig);
        if (ZLGApi.VCI_InitCAN(UintDeviceType, deviceIndex, canIndex, ref config) == (uint)OperationStatus.Failure)
            throw new CanDeviceOperationException();
        initConfig = StructConverter.Converter(config);

        ZLGApi.VCI_ClearBuffer(UintDeviceType, deviceIndex, canIndex);

        base.Connect();
    }
}