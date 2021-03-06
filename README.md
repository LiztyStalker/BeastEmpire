# BeastChess
## 1. 개요
* 블록으로 이루어진 전장에 분대를 배치하고 명령에 따라 전투를 벌이는 턴 베이스 전략 게임
## 2. 개발인원 및 포지션
+ 개인 프로젝트
## 3. 개발환경
+ Unity 2020.3 LTS
+ C#
+ Windows 10
## 4. 사용기술
+ Unity Editor
+ Spine
+ Unity Test Framework
+ AssetBundle
## 5. 현재버전
* Prototype 2 - [2021.10.29]
  + Prototype 2 지인들에게 배포 및 피드백 [2021.10]
	  - 개선사항 발생, 차기 Prototype 3를 위해서 추가 작업 내용 작성
	  - 리팩토링 및 기획 진행 후 차기 Prototype 3 진행 예정 
  + Prototype 1 - [2021.04.24]
## 6. 사용기술
+ AssetBundle을 활용하여 데이터 통합 및 데이터 저장소 시스템 구현
+ TestFramework를 활용하여 PlayMode, EditMode 단위 및 통합 테스트
	- EditMode - 제작한 루틴 단위 테스트 (블록 범위, 위치, 목표 데이터 처리)
	- PlayMode - 제작한 스킬, 병사, 명령, 밸런스 통합 테스트
+ Spine을 활용하여 캐릭터 움직임 구현
+ Unity Editor 및 json을 활용하여 캐릭터, 스킬, 상태이상 데이터 구현
	- json 데이터를 기반으로 자동 병사 생성기 제작 (UnitGenerator)
## 7. 실행방법
+ https://github.com/LiztyStalker/BeastEmpire_Build
## 8. ~~다음버전 - Prototype 3~~ - 중단
* 전장 맵 추가
  + 맵 환경
  + 맵 블록
  + 상태이상
* 상성의 강화 및 병과 추가
  + 돌격병, 기갑병
  + 세력 및 부족의 병과의 분할화
  + 공중 병과 추가
* 세력 추가
* UX 및 도움말 적용
  + 플레이 도움말
  + 승리조건, 패배조건
* 그래픽 및 이펙트 추가
  + 병과 이미지 수정
  + 이펙트 추가
* 지휘관 역할 추가
  + 스킬
  + 전문가
  + 활용도
* 데이터 생성기 추가
  + 스킬
  + 상태이상
  + 지휘관
* 배치 AI 알고리즘 적용
  + ML-Agent 활용
* 밸런스 테스트


