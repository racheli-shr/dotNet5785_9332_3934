﻿using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalApi;

public interface ICrud<T> where T : class
{
    T? Read(int id);
    void Create(T item); //Creates new entity object in DAL
   // T? Read(int id); //Reads entity object by its ID
    IEnumerable<T> ReadAll(Func<T, bool>? filter = null);
    void Update(T item); //Updates entity object
    void Delete(int id); //Deletes an object by its Id
    void DeleteAll(); //Delete all entity objects
    T? Read(Func<T, bool> filter); // stage 2
}

