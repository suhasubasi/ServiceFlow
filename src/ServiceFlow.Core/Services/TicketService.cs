using ServiceFlow.Core.Entities;

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
}