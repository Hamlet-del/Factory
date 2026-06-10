using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Simulator S = new Simulator();
        S.Simulate();

    }
}

public class Simulator
{
    public int ItemId { get; private set; }
    public string ItemType { get; private set; }

    int id = 1;

    public int Apass = 0;
    public int Bpass = 0;
    public int Cpass = 0;

    public int OrderCopasity = 5;
    public int Copasity = 0;

    public int Tick = 0;
    public int TransportTick = 0;
    public record Item(string ItemType, int ItemId);

    List<Item> FactA = new();
    List<Item> FactB = new();
    List<Item> FactC = new();
    List<Item> Orderline = new();
    List<Item> Storage = new();
    List<Item> Transport = new();
    List<Item> Stock = new();

    private void TryMoveToOrderline()
    {
        while (Copasity < OrderCopasity)
        {
            if (FactA.Count > 0 && FactA[0].ItemId == id - (FactA.Count + FactB.Count + FactC.Count))
            {
                Orderline.Add(FactA[0]);
                FactA.RemoveAt(0);
                Copasity++;
            }
            else if (FactB.Count > 0 && FactB[0].ItemId == id - (FactA.Count + FactB.Count + FactC.Count))
            {
                Orderline.Add(FactB[0]);
                FactB.RemoveAt(0);
                Copasity++;
            }
            else if (FactC.Count > 0 && FactC[0].ItemId == id - (FactA.Count + FactB.Count + FactC.Count))
            {
                Orderline.Add(FactC[0]);
                FactC.RemoveAt(0);
                Copasity++;
            }
            else
            {
                break;
            }
        }
    }

    public void GenerateA()
    {
        ItemType = "A";
        ItemId = id;
        id++;
        FactA.Add(new Item(ItemType, ItemId));
        TryMoveToOrderline();
    }

    public void GenerateB()
    {
        ItemType = "B";
        ItemId = id;
        id++;
        FactB.Add(new Item(ItemType, ItemId));
        TryMoveToOrderline();
    }

    public void GenerateC()
    {
        ItemType = "C";
        ItemId = id;
        id++;
        FactC.Add(new Item(ItemType, ItemId));
        TryMoveToOrderline();
    }

    public void QualityChecker()
    {
        if (Orderline.Count == 0) return;

        Random random = new Random();
        int rand = random.Next(0,2);

        Item CurrentItem = Orderline[0];

        if (rand == 0)
        {
            Orderline.RemoveAt(0);
            Copasity--;
        }
        else
        {
            Storage.Add(CurrentItem);
            Orderline.RemoveAt(0);
            Copasity--;

            if (CurrentItem.ItemType == "A") Apass++;
            if (CurrentItem.ItemType == "B") Bpass++;
            if (CurrentItem.ItemType == "C") Cpass++;
        }

        TryMoveToOrderline();
    }

    public void TransportItems()
    {
        TransportTick++;

        if (TransportTick == 4)
        {
            Stock.AddRange(Transport);
            Transport.Clear();
        }

        else if (TransportTick == 8)
        {
            Transport.AddRange(Storage);
            Storage.Clear();
            TransportTick = 0;
        }
    }




    public void PrintFact()
    {
        Console.Write("| FactA: ");
        if (FactA.Count > 0)
            Console.Write($"{FactA[0].ItemType}.{FactA[0].ItemId} ");
        else
            Console.Write("empty");

        Console.Write("| FactB: ");
        if (FactB.Count > 0)
            Console.Write($"{FactB[0].ItemType}.{FactB[0].ItemId} ");
        else
            Console.Write("empty");

        Console.Write("| FactC: ");
        if (FactC.Count > 0)
            Console.Write($"{FactC[0].ItemType}.{FactC[0].ItemId} ");
        else
            Console.Write("empty");

        Console.WriteLine("|");
    }

    public void PrintOrderLine()
    {
        Console.Write("OrderLine ( ");
        foreach (var Item in Orderline)
        {
            Console.Write($"{Item.ItemType}.{Item.ItemId}|");
        }
        Console.WriteLine(" )");
    }

    public void PrintStorage()
    {
        Console.Write("Storage ( ");
        foreach (var Item in Storage)
        {
            Console.Write($"{Item.ItemType}.{Item.ItemId}|");
        }
        Console.WriteLine(" )");
    }

    public void PrintTransport()
    {
        int i = 0;

        if (TransportTick <= 4)
        {
            while (i < TransportTick)
            {
                Console.WriteLine();
                i++;
            }
        }
        else
        {
            while (i < 8 - TransportTick)
            {
                Console.WriteLine();
                i++;
            }

        }

        Console.Write("Car ( ");
        foreach (var Item in Transport)
        {
            Console.Write($"{Item.ItemType}.{Item.ItemId}|");
        }
        Console.Write(" )");

        while (i < 4)
        {
            Console.WriteLine();
            i++;
        }
        Console.WriteLine();
    }

    public void PrintStock()
    {
        Console.Write("Stock ( ");
        foreach (var Item in Stock)
        {
            Console.Write($"{Item.ItemType}.{Item.ItemId}|");
        }
        Console.WriteLine(" )");
    }

    public void PrintAll()
    {
        PrintFact();
        PrintOrderLine();
        PrintStorage();
        PrintTransport();
        PrintStock();
    }

    public void Simulate()
    {
        Console.WriteLine("How much do you want to produce?");
        int limit = int.Parse(Console.ReadLine());

        while (id < limit)
        {
            GenerateA();
            GenerateB();
            GenerateC();

            QualityChecker();
            TransportItems();

            Console.Clear();

            PrintAll();

            System.Threading.Thread.Sleep(500);
            Tick++;
        }

        while (Orderline.Count > 0 || Storage.Count > 0 || Transport.Count > 0)
        {
            QualityChecker();
            TransportItems();

            Console.Clear();

            PrintAll();

            System.Threading.Thread.Sleep(500);
            Tick++;
        }

        Console.WriteLine();
        Console.Write($"Items created {id - 1}, A type pased {Apass}, B type pased {Bpass}, C type pased {Cpass}, Tickes take {Tick} ");
    }

}