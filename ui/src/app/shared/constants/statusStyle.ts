import { UserStatus } from "../enums/UserStatus";

export const USER_STATUS_STYLES = {
    [UserStatus.Blocked]: {
        classname: "c-red",
    },
    [UserStatus.Registered]: {
        classname: "c-green",
    },
}
