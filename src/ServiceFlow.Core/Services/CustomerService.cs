using ServiceFlow.Core.Entities;

namespace ServiceFlow.Core.Services;

public class CustomerService
{
    private List<Customer> customers = new List<Customer>();

    public void Add(Customer customer)
    {
        customers.Add(customer);
    }

    public List<Customer> GetAll()
    {
        return customers;
    }

    public Customer? GetById(Guid id)
    {
        for (int i = 0; i < customers.Count; i++)
        {
            if (customers[i].Id == id)
            {
                return customers[i];
            }
        }

        return null;
    }

    public Customer? GetByEmail(string email)
    {
        for (int i = 0; i < customers.Count; i++)
        {
            if (customers[i].Email== email)
            {
                return customers[i];
            }
        }

        return null;
    }
}