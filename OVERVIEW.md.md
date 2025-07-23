

## Project Architecture Overview

### 1. High-Level Summary  
The project is a **workshop-based configuration** for **n8n**, a visual automation tool for creating workflows. It includes multiple workflow examples using **n8n nodes** for handling **Line Messaging API** interactions, **AI agent integration**, **database operations**, and **custom code execution**. The architecture is built around **Docker** for containerization and **PostgreSQL** for data storage, with a focus on **chatbot automation** and **workflow orchestration**.

---

### 2. Core Components  

*   **Component Name:** `Building_Blocks.json`  
    *   **Purpose:** Demonstrates a basic workflow structure with nodes such as manual triggers, HTTP requests, conditions, and output handling. It is used to illustrate fundamental concepts of workflow building in n8n.  

*   **Component Name:** `Line_Messaging_Chat_Bot.json`  
    *   **Purpose:** Implements a **Line Messaging Chatbot** workflow that processes incoming messages from the Line API, routes them through AI agents, and sends responses using the Line API. It also includes **PostgreSQL integration** for storing chat history and **search tool** integration for retrieving information.  

*   **Component Name:** `Line_Messaging_Chat__basic_.json`  
    *   **Purpose:** Represents a **basic Line Messaging Chatbot** workflow that demonstrates how to handle user input, use AI models like **Ollama** and **OpenRouter**, and reply using the Line API. It is a simplified version of the full chatbot workflow.  

*   **Component Name:** `Dockerfile`  
    *   **Purpose:** Defines the **Docker image** used to run the n8n application. It installs required dependencies and sets up the environment for the n8n workflows.  

*   **Component Name:** `compose.yaml`  
    *   **Purpose:** Describes the **Docker Compose** configuration for running the **n8n** application and **PostgreSQL** database in a local development environment. It sets up the necessary services, ports, and volumes.  

---

### 3. Technology Stack  

*   **Backend:**  
  * **n8n** (workflow automation engine)  
  * **Docker** (containerization)  
  * **PostgreSQL** (database for storing chat history and workflow data)  
  * **Node.js** (underlying runtime for n8n)  
  * **n8n-nodes-base** (core node types for HTTP requests, conditions, etc.)  
  * **@n8n/n8n-nodes-langchain** (integration with AI models like Ollama and OpenRouter)  
  * **n8n-nodes-mcp** (integration with third-party tools and search APIs)  

*   **Frontend:**  
  * **Line Messaging API** (used for chatbot communication)  
  * **n8n Web UI** (for visual workflow creation and management)  

*   **Database:**  
  * **PostgreSQL** (used for storing chat session data and user history)  

*   **API Layer:**  
  * **Line Messaging API** (for sending and receiving messages)  
  * **HTTP REST API** (for integration with external services and AI models)  

*   **Testing:**  
  * **Manual Testing** (via n8n’s UI and workflow execution)  
  * **No automated testing framework** is present in the provided files.  

---

### 4. Key Architectural Patterns & Concepts  

*   **Workflow-Based Architecture:** The project is built using **n8n's declarative workflow model**, which allows users to define complex automation logic through a **visual interface** and a **JSON-based configuration format**.  

*   **Modular Components:** Workflows are modular, with each node representing a discrete function (e.g., HTTP requests, AI model calls, PostgreSQL operations, and message routing).  

*   **AI Integration:** AI models such as **Ollama Chat Model**, **OpenRouter Chat Model**, and **LangChain Agents** are used for natural language processing and chatbot responses.  

*   **Database Integration:** The **PostgreSQL** database is used to store **chat session data**, and it is integrated through the **n8n-nodes-postgres** node type.  

*   **Containerization:** The application is **Dockerized** using a `Dockerfile` and `docker-compose.yaml`, enabling easy deployment and environment consistency across development and production.  

*   **External API Integration:** The project integrates with **Line Messaging API**, **search tools**, and **AI models** to provide a complete chatbot solution.