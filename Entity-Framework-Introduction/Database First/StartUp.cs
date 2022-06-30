using System;
using SoftUni.Data;
using SoftUni.Models;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
               
        }

        //03
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees.Select(x => new
            {
                x.EmployeeId,
                x.FirstName,
                x.LastName,
                x.MiddleName,
                x.JobTitle,
                x.Salary
            }).ToList();
            var sb = new StringBuilder();

            foreach (var item in employees.OrderBy(x => x.EmployeeId))
            {
                sb.AppendLine($"{item.FirstName} {item.LastName} {item.MiddleName} {item.JobTitle} {item.Salary:F2}");
            }

            return sb.ToString().TrimEnd();

        }

        //04
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {

            var sb = new StringBuilder();
            var employees = context.Employees.Where(x => x.Salary > 50_000).Select(x => new { x.FirstName, x.Salary }).ToList();
            foreach (var item in employees.OrderBy(x => x.FirstName))
            {
                sb.AppendLine($"{item.FirstName} - {item.Salary:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        //05
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var employees = context.Employees.Where(x => x.Department.Name.Equals("Research and Development")).Select(x => new { x.FirstName, x.LastName, x.Salary, depName = x.Department.Name }).ToList().OrderBy(x => x.Salary).ThenByDescending(x => x.FirstName);

            foreach (var item in employees)
            {
                sb.AppendLine($"{item.FirstName} {item.LastName} from {item.depName} - ${item.Salary:F2}");
            }
            return sb.ToString().TrimEnd();

        }

        //06
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var sb = new StringBuilder();
            var newAddress = new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            context.Addresses.Add(newAddress);
            context.SaveChanges();

            var nakov = context.Employees.FirstOrDefault(x => x.LastName == "Nakov");
            nakov.AddressId = newAddress.AddressId;

            context.SaveChanges();

            var employeesAddresses = context.Employees.Select(x => new { x.Address.AddressId, x.Address.AddressText }).ToList().OrderByDescending(x => x.AddressId).Take(10);
            foreach (var item in employeesAddresses)
            {
                sb.AppendLine($"{item.AddressText}");
            }

            return sb.ToString().TrimEnd();
        }
        //07
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {

            var sb = new StringBuilder();

            var employess = context.Employees
                .Include(x => x.EmployeesProjects)
                .ThenInclude(x => x.Project)
                .Where(x => x.EmployeesProjects.Any(y => y.Project.StartDate.Year >= 2001 && y.Project.StartDate.Year <= 2003))
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    ManagerFirstName = x.Manager.FirstName,
                    ManagerLastName = x.Manager.LastName,
                    Projects = x.EmployeesProjects.Select(y => new
                        {
                            ProjectName = y.Project.Name,
                            StartDate = y.Project.StartDate,
                            EndDate = y.Project.EndDate
                        }
                    )
                })
                .Take(10)
                .ToList();



            foreach (var item in employess)
            {
                sb.AppendLine($"{item.FirstName} {item.LastName} - Manager: {item.ManagerFirstName} {item.ManagerLastName}");

                foreach (var emproject in item.Projects)
                {
                    var endDate = (emproject.EndDate.HasValue == true) ? emproject.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture) : "not finished";
                    var startDate = (emproject.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture));




                    sb.AppendLine($"--{emproject.ProjectName} - {startDate} - {endDate}");
                }
            }

            return sb.ToString().TrimEnd();

        }
        //08
        public static string GetAddressesByTown(SoftUniContext context)
        {

            var addresses = context.Addresses
                .Include(x => x.Town)
                .OrderByDescending(x => x.Employees.Count)
                .ThenBy(x => x.Town.Name)
                .ThenBy(x => x.AddressText)
                .Select(x => new
                {
                    x.AddressText,
                    TownName = x.Town.Name,
                    x.Employees
                })
                .Take(10)
                .ToList();

            var sb = new StringBuilder();
            foreach (var item in addresses)
            {
                sb.AppendLine($"{item.AddressText}, {item.TownName} - {item.Employees.Count} employees");
            }
            return sb.ToString().TrimEnd();
        }
        //09
        public static string GetEmployee147(SoftUniContext context)
        {

            var employee147 = context.Employees.Include(x => x.EmployeesProjects).ThenInclude(x => x.Project)
                .Select(x => new
                    {
                        x.FirstName,
                        x.LastName,
                        x.EmployeeId,
                        x.JobTitle,
                        ProjectNames = x.EmployeesProjects.Select(y => y.Project.Name).OrderBy(p => p).ToList()



                    }
                )
                .ToList()
                .Where(x => x.EmployeeId == 147);


            var sb = new StringBuilder();
            foreach (var item in employee147)
            {
                sb.AppendLine($"{item.FirstName} {item.LastName} - {item.JobTitle}");


                sb.AppendLine(string.Join(Environment.NewLine, item.ProjectNames));

            }


            return sb.ToString().TrimEnd();
        }
        //10
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {

            var departments = context.Departments
                    .Include(x => x.Employees)
                    .Include(x => x.Manager)
                    .Where(x => x.Employees.Count > 5)
                    .Select(x => new
                    {
                        List = x.Employees.
                            Select(y => new { y.FirstName, y.LastName, y.JobTitle })
                            .OrderBy(y => y.FirstName)
                            .ThenBy(y => y.LastName)
                            .ToList(),
                        x.Manager.FirstName,
                        x.Manager.LastName,
                        x.Name

                    })

                    .ToList()

                ;


            var sb = new StringBuilder();
            foreach (var item in departments.OrderBy(x => x.List.Count))
            {
                sb.AppendLine($"{item.Name} - {item.FirstName} {item.LastName}");
                foreach (var person in item.List)
                {
                    sb.AppendLine($"{person.FirstName} {person.LastName} - {person.JobTitle}");
                }
            }


            return sb.ToString().TrimEnd();
        }
        //11
        public static string GetLatestProjects(SoftUniContext context)
        {

            var sb = new StringBuilder();
            var projects = context.Projects.Select(x => new { x.Name, x.Description, x.StartDate })
                .OrderByDescending(x => x.StartDate)
                .Take(10)
                .ToList();



            foreach (var item in projects.OrderBy(x => x.Name))
            {

                sb.AppendLine($"{item.Name}");
                sb.AppendLine($"{item.Description}");
                sb.AppendLine($"{item.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}");

            }
            return sb.ToString().TrimEnd();

        }
        //12

        //13

        //14

        //15

    }
}
