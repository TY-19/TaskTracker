export interface UserProfile {
    id: string;
    userName: string;
    email: string;
    roles: string[];
    employeeId?: number;
    firstName: string;
    lastName: string;
    boardsIds: number[];
    assignmentsIds: number[];
    [key: string]: any;
}