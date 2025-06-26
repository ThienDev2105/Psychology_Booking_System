
document.addEventListener("DOMContentLoaded", function () {
  // DOM Elements
  const chatbotToggle = document.getElementById("chatbot-toggle");
  const chatbotContainer = document.getElementById("chatbot-container");
  const chatbotClose = document.getElementById("chatbot-close");
  const chatbotMessages = document.getElementById("chatbot-messages");
  const chatbotInput = document.getElementById("chatbot-input-field");
  const chatbotSend = document.getElementById("chatbot-send");
  const suggestionButtons = document.querySelectorAll(".suggestion-btn");

  // Default welcome message when chat opens
  function showWelcomeMessage() {
    setTimeout(() => {
      addMessage("Xin chào! Tôi có thể giúp gì cho bạn hôm nay?", "bot", true);
    }, 500);
  }

  // Toggle chatbot visibility
  chatbotToggle.addEventListener("click", function () {
    chatbotContainer.style.display = "flex";
    chatbotToggle.style.display = "none";

    // Show welcome message if chat is empty
    if (chatbotMessages.children.length === 0) {
      showWelcomeMessage();
    }
  });

  // Close chatbot
  chatbotClose.addEventListener("click", function () {
    chatbotContainer.style.display = "none";
    chatbotToggle.style.display = "flex";
  });

  // Send message with Enter key
  chatbotInput.addEventListener("keypress", function (e) {
    if (e.key === "Enter") {
      sendMessage();
    }
  });

  // Send message with button
  chatbotSend.addEventListener("click", sendMessage);

  // Handle suggestion buttons
  suggestionButtons.forEach((button) => {
    button.addEventListener("click", function () {
      const question = this.getAttribute("data-question");
      addMessage(question, "user");
      processUserMessage(question);
    });
  });

  // Add message to chat
  function addMessage(text, sender, isFirst = false) {
    const messageDiv = document.createElement("div");
    messageDiv.classList.add("message", sender);

    // Add avatar for bot messages
    if (sender === "bot" && isFirst) {
      messageDiv.classList.add("with-avatar");
      const avatarDiv = document.createElement("div");
      avatarDiv.classList.add("bot-avatar");
      const avatarImg = document.createElement("img");
      avatarImg.src = "/image/Logo/LogoSere.jpg";
      avatarDiv.appendChild(avatarImg);
      messageDiv.appendChild(avatarDiv);
    }

    // Message content
    messageDiv.innerHTML += text;

    // Add timestamp
    const timeSpan = document.createElement("span");
    timeSpan.classList.add("message-time");
    const now = new Date();
    timeSpan.textContent =
      now.getHours() + ":" + now.getMinutes().toString().padStart(2, "0");
    messageDiv.appendChild(timeSpan);

    chatbotMessages.appendChild(messageDiv);
    chatbotMessages.scrollTop = chatbotMessages.scrollHeight;
  }

  // Send message function
  function sendMessage() {
    const message = chatbotInput.value.trim();
    if (message) {
      addMessage(message, "user");
      chatbotInput.value = "";

      processUserMessage(message);
    }
  }

  // Process user message and generate response using API
  async function processUserMessage(message) {
    try {
      // Fallback to client-side responses for now
      let botReply;
      const lowerMessage = message.toLowerCase();

      if (
        lowerMessage.includes("làm sao") &&
        lowerMessage.includes("kiểm tra tâm lý")
      ) {
        botReply =
          'Để thực hiện kiểm tra tâm lý:<br/>• Bấm vào mục "Kiểm tra tâm lý" tại đầu trang hoặc "Kiểm tra sức khỏe nhận thức"';
      } else if (lowerMessage.includes("tâm sự")) {
        botReply =
          "Tất nhiên! Tôi luôn ở đây để lắng nghe bạn. Bạn có thể chia sẻ bất cứ điều gì đang khiến bạn lo lắng hoặc buồn phiền.";
      } else if (lowerMessage.includes("sức khỏe tâm lý")) {
        botReply =
          "Chúng tôi có các chuyên gia tâm lý giàu kinh nghiệm. Bạn có thể xem danh sách các bác sĩ và đặt lịch tư vấn trực tiếp tại mục 'Danh sách bác sĩ' trên menu chính.";
      } else if (lowerMessage.includes("podcast")) {
        botReply =
          "Chúng tôi có nhiều podcast mới về sức khỏe tâm lý. Bạn có thể truy cập mục 'Podcast' để nghe các bài nói chuyện về quản lý stress, lo âu và nhiều chủ đề khác.";
      } else if (
        lowerMessage.includes("chào") ||
        lowerMessage.includes("hi") ||
        lowerMessage.includes("hello")
      ) {
        botReply = "Xin chào! Tôi tên Diamont có thể giúp gì cho bạn hôm nay?";
      } else {
        botReply = "Tôi có thể giúp gì cho bạn hôm nay?";
      }

      setTimeout(() => {
        addMessage(botReply, "bot");
      }, 800);

      // Attempt API call if available
      try {
        const response = await fetch("/api/chatbot/query", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ message: message }),
        });

        if (response.ok) {
          const data = await response.json();
          // We already showed a fallback response, so we won't use this unless needed
          // addMessage(data.response, "bot");
        }
      } catch (apiError) {
        console.error("API error:", apiError);
        // Already showed fallback response
      }
    } catch (error) {
      console.error("Error processing message:", error);
      setTimeout(() => {
        addMessage("Xin lỗi, có lỗi xảy ra. Vui lòng thử lại.", "bot");
      }, 800);
    }
  }

});
