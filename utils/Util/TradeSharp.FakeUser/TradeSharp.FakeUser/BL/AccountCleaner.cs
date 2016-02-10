using System;
using System.Linq;
using TradeSharp.Contract.Entity;
using TradeSharp.Linq;

namespace TradeSharp.FakeUser.BL
{
    public class AccountCleaner
    {
        public bool CloseAccount(int accountId, TradeSharpConnection ctx)
        {
            var ac = ctx.ACCOUNT.FirstOrDefault(a => a.ID == accountId);
            if (ac == null) return false;
            CloseOrders(ac, ctx);
            ZeroBalance(ac, ctx);
            return true;
        }

        private void CloseOrders(ACCOUNT ac, TradeSharpConnection ctx)
        {
            var ordersToClose = ctx.POSITION.Where(p => p.AccountID == ac.ID).ToList();
            foreach (var order in ordersToClose)
                ctx.POSITION.Remove(order);

            var pendingsToClose = ctx.PENDING_ORDER.Where(p => p.AccountID == ac.ID).ToList();
            foreach (var order in pendingsToClose)
                ctx.PENDING_ORDER.Remove(order);

            ctx.SaveChanges();
        }

        private void ZeroBalance(ACCOUNT ac, TradeSharpConnection ctx)
        {
            if (ac.Balance == 0) return;

            var valueDate = DateTime.Now;
            var amount = ac.Balance;
            ac.Balance = 0;

            var bc = new BALANCE_CHANGE
            {
                AccountID = ac.ID,
                Amount = amount,
                ChangeType = (int) BalanceChangeType.Withdrawal,
                Description = "closing account",
                ValueDate = valueDate
            };

            ctx.BALANCE_CHANGE.Add(bc);
            ctx.SaveChanges();
        }
    }
}
