﻿using System;

namespace StudyHard.Domain
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Active { get; set; }

        public bool CourseType { get; set; }
    }
}