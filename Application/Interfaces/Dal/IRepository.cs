using System.Transactions;

namespace Application.Interfaces.Dal;

public interface IRepository
{
    TransactionScope CreateTransactionScope();
}