import { DifficultyLevel } from "../../Enums/DifficultyLevel"

export interface UserTestsDto {
    difficultyLevel: DifficultyLevel;
    passed: boolean;
}