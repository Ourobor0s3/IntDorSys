import { Pipe, PipeTransform } from '@angular/core';
import { UserStatus } from "../enums/UserStatus";
import { USER_STATUS_STYLES } from "../constants/statusStyle";

@Pipe({
    name: 'statusStyle',
})
export class StatusStylePipe implements PipeTransform {
    transform(status: UserStatus): { classname: string } {
        return USER_STATUS_STYLES[status];
    }
}
