import { Employee } from "../employee";
import { Stage } from "../stage";

export interface AssignmentDisplayModel {
    id: number;
    topic: string;
    description?: string;
    deadline?: Date;
    isCompleted?: boolean;
    stage?: Stage;
    responsibleEmployee?: Employee;
    [key: string]: any;
}