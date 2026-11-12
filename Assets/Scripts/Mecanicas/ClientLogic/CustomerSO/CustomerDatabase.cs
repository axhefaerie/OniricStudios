using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomerDatabase", menuName = "Scriptable Objects/CustomerDatabase")]
public class CustomerDatabase : ScriptableObject
{
    public List<CustomerType> allCustomers;

    public CustomerType FindByName(string name)
    {
        return allCustomers.Find(c => c.customerName == name);
    }
}
