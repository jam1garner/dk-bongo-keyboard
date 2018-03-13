namespace DkBongoKeyboard
{
    internal class MessageFactory
    {
        internal const int DkBongoId = 0x1844;

        public static DkBongoMessage CreateMessage(int productId, byte[] messageData)
        {
            switch (productId)
            {
                case DkBongoId: return new DkBongoMessage(messageData);
            }
            return null;
        }
    }
}
