# Imuniza - Jogo Educativo sobre Imunização

Um jogo educativo desenvolvido em Unity que simula a gestão de saúde pública e o controle de surtos de doenças através da vacinação.

## Sobre o Projeto

O **Imuniza** é um jogo de simulação onde o jogador assume o papel de um gestor de saúde pública responsável por controlar surtos de doenças infecciosas em uma cidade. O objetivo é manter a população saudável através de:

- Vacinação preventiva
- Diagnóstico no laboratório
- Tratamento hospitalar
- Gerenciamento de recursos financeiros

### Doenças Simuladas

- **Dengue** - Transmitida por mosquitos
- **Zika** - Vírus Zika
- **Gripe** - Influenza
- **Sarampo** - Altamente contagioso
- **Hepatite** - Várias formas
- **Febre Amarela** - Endêmica no Brasil

## Requisitos

- **Unity 2019.3.7f1** ou superior (recomendado Unity 6.3 LTS - 6000.3.6f1)
- **Sistema Operacional**: Windows, macOS ou Linux

## Instalação

### 1. Clonar o Repositório

```bash
git clone https://github.com/makaires77/imuniza.git
cd imuniza
```

### 2. Abrir no Unity

1. Abra o **Unity Hub**
2. Clique em **Add** → **Add project from disk**
3. Selecione a pasta do projeto
4. Se solicitado, confirme a atualização da versão do Unity

### 3. Executar o Jogo

1. No Unity, abra a cena `Assets/Scenes/Menu.unity`
2. Pressione **Play** (▶) para testar

## Estrutura do Projeto

```
Assets/
├── Animation/          # Animações e controllers
├── Art/                # Assets gráficos 2D/3D
├── Prefabs/            # Prefabs do jogo
├── Resources/          # Dados de doenças, vacinas e sintomas
├── Scenes/             # Cenas do jogo
│   ├── Menu.unity      # Menu principal
│   ├── Cidade.unity    # Cena principal do jogo
│   └── SplashScreen.unity
├── Scripts/            # Código fonte C#
│   ├── Core/           # Sistemas centrais
│   ├── Data/           # ScriptableObjects
│   ├── IA/             # Inteligência artificial dos NPCs
│   ├── Statistics/     # Dashboard e estatísticas
│   └── UI/             # Interface do usuário
└── Sound-Music/        # Áudio e música
```

## Arquitetura do Sistema

### Sistema de Tempo

O jogo utiliza um sistema de tempo customizado que permite:
- Pausar/retomar o jogo
- Velocidades: 0.5x, 1x, 2x
- Ciclo dia/noite
- Eventos agendados por dia/segundo

### Ciclo de Vida dos Personagens

```
SAUDÁVEL → DOENTE → TRATAMENTO → CURADO (com vacina)
                  ↓
               MORTO
```

### Sistemas Principais

| Sistema | Arquivo | Descrição |
|---------|---------|-----------|
| Tempo | `ClockBehaviour.cs` | Controle do relógio e eventos |
| Personagens | `CharactersManager.cs` | Spawn e gerenciamento de NPCs |
| Doenças | `Disease.cs` | Transmissão e contágio |
| Hospital | `Hospital.cs` | Tratamento e cura |
| Laboratório | `Laboratory.cs` | Diagnóstico |
| Economia | `MoneyManager.cs` | Sistema financeiro |

## Gameplay

### Objetivo

Vacinar toda a população antes que as doenças se espalhem e causem mortes.

### Controles

- **Clique** - Selecionar personagem ou edificação
- **Scroll** - Zoom da câmera
- **Arraste** - Mover câmera

### Mecânicas

1. **Transmissão de Doenças**
   - Doenças se espalham por zonas de contágio
   - Contato pessoa-a-pessoa também transmite
   - Cadáveres infectados continuam contagiosos

2. **Sistema Imunológico**
   - Cada personagem tem imunidade de 40-80%
   - Vacinas conferem imunidade permanente
   - Chance de transmissão = Transmissibilidade - Imunidade

3. **Tratamentos**
   - **Hospital**: 4 dias, R$ 80 - Cura + Vacina
   - **Laboratório**: 1 dia, R$ 15 - Diagnóstico
   - **Posto de Saúde**: 1 dia, R$ 10 - Vacina preventiva

4. **Economia**
   - Início: R$ 400
   - Ganho diário: R$ 10 por pessoa saudável
   - Custos: Tratamentos e vacinas

## Desenvolvimento

### Tecnologias

- **Engine**: Unity 2019.3.7f1+
- **Linguagem**: C#
- **IA**: NavMesh para pathfinding
- **UI**: Unity UI + TextMesh Pro

### Padrões de Design

- **Singleton**: GameManager, MoneyManager
- **ScriptableObject**: Dados de doenças/vacinas/sintomas
- **Observer**: Sistema de eventos de tempo
- **State Machine**: Estados de saúde dos personagens

### Contribuindo

1. Fork o repositório
2. Crie uma branch: `git checkout -b feature/nova-funcionalidade`
3. Commit suas mudanças: `git commit -m 'Adiciona nova funcionalidade'`
4. Push para a branch: `git push origin feature/nova-funcionalidade`
5. Abra um Pull Request

## Bugs Conhecidos e Correções Recentes

### Corrigidos (v1.1)

- ✅ Divisão por zero no sistema de tempo quando pausado
- ✅ NullReferenceException no delegate de verificação de morte
- ✅ Memory leak em eventos de tempo passados
- ✅ Conflito entre sistemas de pausa (Time.timeScale removido)
- ✅ Performance melhorada com cache de personagens
- ✅ Laboratório agora funcional

### Em Desenvolvimento

- [ ] Sistema de tutorial
- [ ] Mais doenças e vacinas
- [ ] Modo campanha com níveis
- [ ] Localização para outros idiomas

## Licença

Este projeto é desenvolvido para fins educacionais.

## Créditos

Desenvolvido com Unity e muito café ☕

---

**FIOCE** - Fundação Oswaldo Cruz Escritório Ceará
