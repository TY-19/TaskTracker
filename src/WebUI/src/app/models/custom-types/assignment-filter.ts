import { Assignment } from "../assignment";

export type AssignmentFilter = (a: Assignment, employeeId?: number) => boolean;