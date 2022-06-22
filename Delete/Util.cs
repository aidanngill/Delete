namespace Delete
{
    internal class Util
    {
        public static string DeleteOrCount(bool x) => x ? "counting" : "deleting";
        public static async Task WaitRandomTime(int minTime, int maxTime) => await Task.Delay(new Random().Next(minTime, maxTime));
    }
}
