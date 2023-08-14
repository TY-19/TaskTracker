import { Assignment } from "src/app/models/assignment";

export function filterMyIncompletedTasks(a: Assignment, employeeId?: number): boolean {
    if (!a || !employeeId) 
        return false;
    
    return a.responsibleEmployeeId === employeeId && !a.isCompleted;
}
export function filterAllMyTasks(a: Assignment, employeeId?: number): boolean {
    if (!a || !employeeId) 
        return false;
    
    return a.responsibleEmployeeId === employeeId;
}
export function filterAllIncompletedTasks(a: Assignment, employeeId?: number): boolean {
    return !a.isCompleted;
}
export function filterAllTasks(a: Assignment, employeeId?: number): boolean {
    return true;
}