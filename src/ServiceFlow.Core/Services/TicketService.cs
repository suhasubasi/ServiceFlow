using ServiceFlow.Core.Entities;
using ServiceFlow.Core.Enums;

namespace ServiceFlow.Core.Services;

public class TicketService
{
    private List<ServiceTicket> tickets = new List<ServiceTicket>();

    public void Add(ServiceTicket ticket)
    {
        tickets.Add(ticket);
    }

    public List<ServiceTicket> GetAll()
    {
        return tickets;
    }

    public ServiceTicket? GetById(Guid id)
    {
        for (int i = 0; i < tickets.Count; i++)
        {
            if (tickets[i].Id == id)
            {
                return tickets[i];
            }
        }

        return null;
    }

    public List<ServiceTicket> GetByStatus(TicketStatus status)
    {
        var results = new List<ServiceTicket>();

        for (int i = 0; i < tickets.Count; i++)
        {
            if (tickets[i].Status == status)
            {
                results.Add(tickets[i]);
            }
        }

        return results;
    }

    public List<ServiceTicket> GetByCustomerId(string customerId)
    {
        var results = new List<ServiceTicket>();

        for (int i = 0; i < tickets.Count; i++)
        {
            if (tickets[i].CustomerId == customerId)
            {
                results.Add(tickets[i]);
            }
        }

        return results;
    }

    public List<ServiceTicket> GetByPriority(TicketPriority priority)
    {
        var results = new List<ServiceTicket>();

        for (int i = 0; i < tickets.Count; i++)
        {
            if (tickets[i].Priority == priority)
            {
                results.Add(tickets[i]);
            }
        }

        return results;
    }

    public bool Remove(Guid id)
    {
        for (int i = 0; i < tickets.Count; i++)
        {
            if (tickets[i].Id == id)
            {
                tickets.RemoveAt(i);
                return true;
            }
        }

        return false;

    }
}