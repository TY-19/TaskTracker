import { AssignmentDisplayModel } from "src/app/models/display-models/assignment-display-model";

export function AssignmentDisplayModelSortingDataAccessor(
    data: AssignmentDisplayModel, sortHeaderId: string): string | number {
      switch(typeof(data[sortHeaderId])) {
        case 'string':
          return data[sortHeaderId].toLowerCase();
        case 'object':
          return SortCustomTypes(data, sortHeaderId);
        default:
          return data[sortHeaderId];
      }
  }

function SortCustomTypes(data: AssignmentDisplayModel, sortHeaderId: string): string | number {
    if (data[sortHeaderId].position)
        return SortStages(data, sortHeaderId);
    if (data[sortHeaderId].firstName || data[sortHeaderId].lastName)
        return SortEmployees(data, sortHeaderId);
    return 0;
}

function SortStages(data: AssignmentDisplayModel, sortHeaderId: string): string | number {
    return data[sortHeaderId].position
}

function SortEmployees(data: AssignmentDisplayModel, sortHeaderId: string)
    : string | number {
    if(data[sortHeaderId].firstName && data[sortHeaderId].lastName)
        return data[sortHeaderId].firstName.toLowerCase() + data[sortHeaderId].lastName.toLowerCase();
    if (data[sortHeaderId].firstName)
        return data[sortHeaderId].firstName.toLowerCase();
    if (data[sortHeaderId].lastName)
        return data[sortHeaderId].lastName.toLowerCase();
    return 0;
}