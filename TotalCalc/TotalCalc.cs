namespace TotalCalc
{

    // Объявление класса
    public class LockTotalCounter
    {
        decimal total = 0;
        object lockObj = new object();
        public decimal Total
        {
            get { return total; }
        }
        public void AddTotal(decimal value)
        {
            lock (lockObj)
            {
                total += value;
            }
        }
        public void DecTotal(decimal value)
        {
            lock (lockObj)
            {
                total -= value;
            }
        }
    }
}