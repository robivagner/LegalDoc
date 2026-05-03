import os
import json
from pathlib import Path
from google import genai
from dotenv import load_dotenv

load_dotenv()

class DocumentAnalyzer:
    def __init__(self):
        api_key = os.getenv("GEMINI_API_KEY")
        if not api_key:
            raise ValueError("GEMINI_API_KEY nu a fost găsit în .env")

        self.client = genai.Client(api_key=api_key)
        self.model_name = "gemini-2.5-flash"

    async def analyze(self, content: str):
        if not content or len(content.strip()) < 10:
            raise ValueError("Conținut insuficient pentru analiză.")

        prompt = f"""
        You are an expert Senior Legal Counsel. Analyze the following legal document and provide a high-level professional assessment.
        
        STRICT OUTPUT RULES:
        1. The response must be a VALID JSON object with keys: "summary", "clauses", "risks".
        2. Values for "clauses" and "risks" MUST be plain strings, NOT arrays.
        3. Use numbered lists (1., 2., 3.) with newline characters (\\n).
        4. Language: SAME as the document.
        5. NO BOILERPLATE: Ignore introductory info, addresses, and standard definitions unless they contain a hidden trap.
        
        FIELDS TO EXTRACT:
        - summary: A 3-5 sentence overview. Mention specifically the reward/benefit and the core condition to get it.
        - clauses: Focus ONLY on "Active Obligations": 
            * Specific deadlines (dates, intervals).
            * Mandatory actions the user must take (opt-ins, income thresholds).
            * Restrictions (what is NOT allowed).
        - risks: Focus ONLY on "High-Impact Issues":
            * Unilateral rights (where the Organizer can change/stop everything without notice).
            * Ambiguous terms (e.g., "reasonable suspicions", "objective criteria").
            * Forfeiture (how the user can lose the prize already earned).
            * Budget caps that can terminate the campaign early.
        
        LEGAL DOCUMENT TO ANALYZE:
        ---
        {content}
        ---
        """

        response = self.client.models.generate_content(
            model=self.model_name,
            contents=prompt,
            config={
                'response_mime_type': 'application/json'
            }
        )

        if not response.text:
            raise Exception("AI-ul a returnat un răspuns gol.")

        return json.loads(response.text)