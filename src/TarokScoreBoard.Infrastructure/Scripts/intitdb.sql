create table game(
  game_id uuid primary key not null,
  name varchar(100) not null,
  date timestamp not null
);

create table round(
  round_id uuid primary key not null,
  game_id uuid not null,
  game_type int,
  difference int,
  won boolean not null,
  lead_player_id uuid,
  supporting_player_id uuid,
  is_klop boolean not null,
  contra_factor int not null,
  mond_fang_player_id uuid,
  pagat_fang_player_id uuid,
  round_number int not null
);

create table game_player(
  player_id uuid not null,
  game_id uuid not null,
  name varchar(100) not null
);

create table round_result(
  game_id uuid not null,
  round_id uuid not null,
  round_score_change int,
  player_id uuid not null,
  player_score int not null,
  player_radelc_count int not null,
  player_radelc_used int not null
);


create table round_modifier(
  round_id uuid not null,
  team int not null default 1,
  announced boolean not null,
  contra int not null default 1,
  modifier_type varchar(64) not null
);

create table team (
  team_id uuid primary key not null,
  team_user_id varchar(255) not null,
  team_name  varchar(255)     not null,
  passphrase varchar(255)     not null  
);

create table team_player (
  team_id   uuid         not null,
  player_id uuid         not null,
  name      varchar(100) not null
);

create table team_access_token(
  team_id uuid not null,
  access_token uuid not null,
  date_issued date not null default now()
);