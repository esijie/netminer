using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetMiner
{
    public static class Constant
    {
        //定义一个远程任务分类
        public const string g_RemoteTaskClass = "远程";
        public const string g_RemoteTaskPath = "tasks\\RemoteTask";

        public const string g_TaskClassFile = "tasks\\TaskClass.xml";
        public const string g_TaskRunFile = "tasks\\TaskRun.xml";
        public const string g_TaskPath = "tasks";
        public const string g_TaskRunPath = "Tasks\\run";
        public const string g_CompletedFile = "Data\\index.xml";

        public const string g_PublishInfoFile = "publish\\PublishInfo.xml";

        public const string g_PlanFile = "tasks\\plan\\plan.xml";

        public const string g_ProxyFile = "tasks\\proxy.db";

        public const string g_LogPath = "log";

        public const string g_RadarFile = "monitor\\index.xml";

        public const string g_TaskDataPath = "data";
    }
}
