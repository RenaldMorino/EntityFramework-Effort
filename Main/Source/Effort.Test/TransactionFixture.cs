﻿#region License

// Copyright (c) 2011 Effort Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort.Test.Data;
using System.Transactions;
using Effort.Test.Utils;

namespace Effort.Test
{
    [TestClass]
    public class TransactionFixture
    {
        private NorthwindEntities context;

        [TestInitialize]
        public void Initialize()
        {
            this.context = new NorthwindEntitiesEmulated();
        }

        [TestMethod]
        public void TransactionScopeRollback()
        {
            Customers customer = new Customers();

            customer.CompanyName = "company";
            customer.CustomerID = "CUSTO";

            context.Connection.Open();

            using (TransactionScope tran = new TransactionScope())
            {
                this.context.Customers.AddObject(customer);
                this.context.SaveChanges();


                bool customerWasAdded = context.Customers.FirstOrDefault(c => c.CustomerID == "CUSTO") != null;
                Assert.IsTrue(customerWasAdded);

                // Omit 'tran.Complete()' to achieve rollback 
            }

            bool customerWasNotAdded = context.Customers.FirstOrDefault(c => c.CustomerID == "CUSTO") == null;
            Assert.IsTrue(customerWasNotAdded);
        }


        [TestMethod]
        public void TransactionScopeCommit()
        {
            Customers customer = new Customers();

            customer.CompanyName = "company";
            customer.CustomerID = "CUSTO";

            using (TransactionScope tran = new TransactionScope())
            {
                context.Customers.AddObject(customer);
                context.SaveChanges();

                tran.Complete();
            }

            bool customerWasAdded = context.Customers.FirstOrDefault(c => c.CustomerID == "CUSTO") != null;
            
            Assert.IsTrue(customerWasAdded);
            
        }
    }
}
