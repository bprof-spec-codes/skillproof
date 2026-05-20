import { Injectable } from '@angular/core';
import { BadgeDto } from '../Models/Dtos/User/badge-dto';

@Injectable({
  providedIn: 'root',
})
export class BadgeService {
  
  getBadgeIconForSkill(skillName: string, badges: BadgeDto[]): string {
    if (!badges || !badges.length) {
      return 'Assets/Unknown.svg';
    }

    const skillBadges = badges.filter(b => b.sourceName === skillName);

    if (!skillBadges.length) {
      return 'Assets/Unknown.svg'; 
    }
    
    const highestBadge = skillBadges.reduce((prev, current) => {
      const prevScore = this.getDifficultyScore(prev.difficultyLevel);
      const currScore = this.getDifficultyScore(current.difficultyLevel);
      return currScore > prevScore ? current : prev;
    });
    
    const levelStr = String(highestBadge.difficultyLevel).toLowerCase();
    
    switch (levelStr) {
      case '0':
      case 'junior':
        return 'Assets/Junior.svg';
        
      case '1':
      case 'medior':
        return 'Assets/Medior.svg';
        
      case '2':
      case 'senior':
        return 'Assets/Senior.svg';
        
      default:
        console.warn('Unknown difficulty level received:', highestBadge.difficultyLevel);
        return 'Assets/Unknown.svg';
    }
  }

  private getDifficultyScore(level: any): number {
    const str = String(level).toLowerCase();
    if (str === '2' || str === 'senior') return 2;
    if (str === '1' || str === 'medior') return 1;
    if (str === '0' || str === 'junior') return 0;
    return -1;
  }
}
