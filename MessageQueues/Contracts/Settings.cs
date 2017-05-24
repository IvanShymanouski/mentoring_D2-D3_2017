namespace Contracts
{
    public static class Settings
    {
        public static string ServerQueueName = @".\private$\CentralServerQueue";
        public static string MonitorQueueName = @".\private$\MonitorQueue";
        public static string ClientQueueName = @".\Private$\ClientQueue";
        
        public static int DefaultTimeOut = 4000;
        public static int ChunkSize = 1024;
    }
}
