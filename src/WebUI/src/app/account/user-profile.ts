export interface UserProfile {
    Id: string;
    UserName: string;
    Email: string;
    EmployeeId?: number;
    FirstName: string;
    LastName: string;
    BoardsIds: number[];
    AssignmentsIds: number[];
}