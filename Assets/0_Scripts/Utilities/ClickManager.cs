namespace _Scripts
{
    public static class ClickManager
    {
        public static bool ClickLock = true;

        public static void LockClick()
        {
            ClickLock = true;
        }

        public static void UnlockClick()
        {
            ClickLock = false;
        }
        
    }
}
