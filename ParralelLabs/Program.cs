// See https://aka.ms/new-console-template for more information

using System;
using DataStructures;
using ParralelLabs.Task3;
using ParralelLabs.Task4;

Console.WriteLine("## SkipList");
SkipList<int> skipList = new(3, 5);
skipList.Add(2);
skipList.Add(3);
Console.WriteLine(skipList);

Console.WriteLine("## Michael and Scott");
MichaelAndScott<int> ms = new();
ms.Enqueue(2);
ms.Enqueue(3);
int res;
ms.TryDequeue(out res);
Console.WriteLine(res);
ms.TryDequeue(out res);
Console.WriteLine(res);

Console.WriteLine("### Harris linked list");
HarrisOrderedList<int> hol = new();
hol.Add(2);
hol.Add(3);
hol.Add(4);
hol.Remove(2);
hol.Remove(4);
Console.WriteLine(hol);
