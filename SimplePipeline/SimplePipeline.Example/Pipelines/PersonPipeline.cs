﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimplePipeline.Example.Filters;
using SimplePipeline.Example.Models;

namespace SimplePipeline.Example.Pipelines
{
    public class PersonPipeline : IPipeline<String, IEnumerable<Person>>
    {
        private readonly IPipeline<String, IEnumerable<Person>> innerPipeline;

        public PersonPipeline()
        {
            innerPipeline = new Pipeline<String, IEnumerable<Person>>()
            {
                new ReadFileFilter(),
                new ParsePersonsFilter(),
                new GroupFilter<Person, String>(person=> person.FirstName),
                new OrderDescendingFilter<IGrouping<String,Person>,int>(group => group.Count()),
                new FirstFilter<IGrouping<String,Person>>()
            };
        }

        public IEnumerator<Object> GetEnumerator()
        {
            return innerPipeline.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<Person> Output
        {
            get
            {
                return innerPipeline.Output;
            }
        }

        public Exception Exception
        {
            get
            {
                return innerPipeline.Exception;
            }
        }

        public Boolean IsBeginState
        {
            get
            {
                return innerPipeline.IsBeginState;
            }
        }

        public Boolean Execute(String input)
        {
            return innerPipeline.Execute(input);
        }

        public void Reset()
        {
            innerPipeline.Reset();
        }
    }
}