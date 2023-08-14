import { Assignment } from "src/app/models/assignment";
import { Employee } from "src/app/models/employee";

export function employeeByFirstNameComparator(firstEmployee: Employee, 
    secondEmployee: Employee) : number {
        let compareResult = firstEmployee.firstName?.localeCompare(secondEmployee.firstName!);
        return compareResult ?? 0;
  };

export function sortTasksByNameAsc(a: Assignment, b: Assignment): number {
    return a.topic.toLowerCase() > b.topic.toLowerCase() ? 1 : -1;
}

export function sortTasksByNameDesc(a: Assignment, b: Assignment): number {
    return a.topic.toLowerCase() > b.topic.toLowerCase() ? -1 : 1;
}

export function sortTasksByDeadlineAsc(a: Assignment, b: Assignment): number {
    if (a.deadline) {
        if (b.deadline)
            return a.deadline > b.deadline ? 1 : -1;
        else
            return 1;
    }
    return b.deadline ? -1 : 0;
}

export function sortTasksByDeadlineDesc(a: Assignment, b: Assignment): number {
    if (a.deadline) {
        if (b.deadline)
            return a.deadline > b.deadline ? -1 : 1;
        else
            return -1;
    }
    return b.deadline ? 1 : 0;
}