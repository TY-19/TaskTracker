import { Employee } from "src/app/models/employee";

export function employeeByFirstNameComparator(firstEmployee: Employee, 
    secondEmployee: Employee) : number {
        let compareResult = firstEmployee.firstName?.localeCompare(secondEmployee.firstName!);
        return compareResult ?? 0;
  };