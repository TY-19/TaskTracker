import { Subpart } from "./subpart";

export interface Assignment {
    id: number;
    topic: string;
    description?: string;
    deadline?: Date;
    isCompleted?: boolean;
    boardId: number;
    stageId: number;
    responsibleEmployeeId: number;
    subparts: Subpart[];
}